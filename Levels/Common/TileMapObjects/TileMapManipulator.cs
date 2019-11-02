using GameOff_2019.Entities.Common.Navigation;
using Godot;
using Godot.Collections;

namespace GameOff_2019.Levels.Common.TileMapObjects {
    public class TileMapManipulator : Node {
        [Export] private readonly NodePath pathfindingTileMapNodePath = null;
        private PathfindingTileMap pathfindingTileMap;
        [Export] private readonly NodePath tileMapObjectContainerNodePath = null;
        private Node2D tileMapObjectContainer;

        // ReSharper disable once CollectionNeverUpdated.Local
        [Export] private readonly Dictionary<int, PackedScene> tileIdToPackedSceneMapping = new Dictionary<int, PackedScene>();

        private readonly Dictionary<int, Node> tileMapObjects = new Dictionary<int, Node>();

        public override async void _Ready() {
            pathfindingTileMap = GetNode<PathfindingTileMap>(pathfindingTileMapNodePath);
            tileMapObjectContainer = GetNode<Node2D>(tileMapObjectContainerNodePath);
            await ToSignal(Owner, "ready");
            SetupTileMapObjects();
        }

        public void ChangeCellToId(Vector2 cellPosition, int newCellId) {
            pathfindingTileMap.SetCell((int) cellPosition.x, (int) cellPosition.y, newCellId);
        }

        private void SetupTileMapObjects() {
            var usedCells = new Array<Vector2>(pathfindingTileMap.GetUsedCells());
            foreach (var usedCell in usedCells) {
                SetupOrReplaceTileMapObject(usedCell);
            }
        }

        public void SetupOrReplaceTileMapObject(Vector2 cell, int newCellId = -1) {
            if (newCellId != -1) {
                pathfindingTileMap.SetCell((int) cell.x, (int) cell.y, newCellId);
            }

            var uniqueTileId = pathfindingTileMap.GetIdForTile(cell);
            var worldPosition = pathfindingTileMap.MapToWorld(cell) + pathfindingTileMap.CellSize / 2;
            var cellId = pathfindingTileMap.GetCell((int) cell.x, (int) cell.y);

            if (!tileIdToPackedSceneMapping.TryGetValue(cellId, out var packedScene)) {
                if (tileMapObjects.TryGetValue(uniqueTileId, out var optionalNode)) {
                    ((ITileMapObject) optionalNode).Object().QueueFree();
                }

                tileMapObjects.Remove(uniqueTileId);
                return;
            }

            switch (cellId) {
                case 1: //Tree
                    var tree = new TreeTileMapObject();
                    tree.Init(cell, worldPosition, packedScene);
                    if (tileMapObjects.TryGetValue(uniqueTileId, out var optionalTreeNode)) {
                        ((ITileMapObject) optionalTreeNode).Object().QueueFree();
                    }

                    tileMapObjects.Add(uniqueTileId, tree);
                    AddTileMapObjectNode(tree);
                    break;
                default:
                    if (tileMapObjects.TryGetValue(uniqueTileId, out var optionalNode)) {
                        ((ITileMapObject) optionalNode).Object().QueueFree();
                    }

                    tileMapObjects.Remove(uniqueTileId);
                    return;
            }

            pathfindingTileMap.UpdateAStarGrid();
        }

        private void AddTileMapObjectNode(ITileMapObject tileMapObject) {
            if (tileMapObject.PackedScene().Instance() is Node2D treeNode) {
                tileMapObject.SetObject(treeNode);
                treeNode.ZIndex = (int) tileMapObject.TileMapPosition().y * 2;
                AddNodeOfTileMapObjectToScene(tileMapObject);
            }

//            Logger.Error("Node with packed Scene " + tileMapObject.PackedScene() + " was null after instantiation!");
        }

        private void AddNodeOfTileMapObjectToScene(ITileMapObject tileMapObject) {
            if (tileMapObject.Object() != null) {
                tileMapObjectContainer.AddChild(tileMapObject.Object());
                tileMapObject.Object().SetPosition(tileMapObject.WorldPosition());
            }
        }
    }
}
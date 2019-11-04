using System;
using GameOff_2019.Entities.Common.Navigation;
using GameOff_2019.Levels.Common.TileMapObjects.TreeObject;
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

        public void SetupOrReplaceTileMapObject(Vector2 cell, int newCellId = -1) {
            if (newCellId != -1) {
                pathfindingTileMap.SetCell((int) cell.x, (int) cell.y, newCellId);
            }

            var uniqueTileId = pathfindingTileMap.GetIdForTile(cell);
            var worldPosition = pathfindingTileMap.MapToWorld(cell) + pathfindingTileMap.CellSize / 2;
            var cellId = pathfindingTileMap.GetCell((int) cell.x, (int) cell.y);

            tileIdToPackedSceneMapping.TryGetValue(cellId, out var packedScene);

            if (packedScene?.Instance() is TileMapObject tileMapObject) {
                if (cellId == pathfindingTileMap.traversableId) {
                    tileMapObject.Init(cell, worldPosition);
                    if (tileMapObjects.TryGetValue(uniqueTileId, out var optionalBaseObjectNode)) {
                        optionalBaseObjectNode.QueueFree();
                    }

                    tileMapObjects.Remove(uniqueTileId);
                    tileMapObjects.Add(uniqueTileId, tileMapObject);
                    AddTileMapObjectNode(tileMapObject);
                }
                else if (cellId == pathfindingTileMap.treeId) {
                    tileMapObject.Init(cell, worldPosition);
                    if (tileMapObjects.TryGetValue(uniqueTileId, out var optionalTreeNode)) {
                        optionalTreeNode.QueueFree();
                    }

                    tileMapObjects.Remove(uniqueTileId);
                    tileMapObjects.Add(uniqueTileId, tileMapObject);
                    AddTileMapObjectNode(tileMapObject);
                    //TODO: change tiles in radius
                }
                else {
                    if (tileMapObjects.TryGetValue(uniqueTileId, out var optionalNode)) {
                        optionalNode.QueueFree();
                    }

                    tileMapObjects.Remove(uniqueTileId);
                }
            }

            pathfindingTileMap.UpdateAStarGrid();
        }

        public TileMapObject GetTileMapObjectWithTileMapCoordinates(Vector2 position) {
            var tileId = pathfindingTileMap.GetIdForTile(position);
            tileMapObjects.TryGetValue(tileId, out var tileMapObject);
            if (tileMapObject is TileMapObject mapObject) {
                return mapObject;
            }

            throw new Exception(tileMapObject + " does not extend ITileMapObject! This should never happen!");
        }


        private void SetupTileMapObjects() {
            var usedCells = new Array<Vector2>(pathfindingTileMap.GetUsedCells());
            foreach (var usedCell in usedCells) {
                SetupOrReplaceTileMapObject(usedCell);
            }
        }

        private void AddTileMapObjectNode(TileMapObject tileMapObject) {
            tileMapObject.ZIndex = (int) tileMapObject.TileMapPosition().y * 2;
            AddNodeOfTileMapObjectToScene(tileMapObject);


//            Logger.Error("Node with packed Scene " + tileMapObject.PackedScene() + " was null after instantiation!");
        }

        private void AddNodeOfTileMapObjectToScene(TileMapObject tileMapObject) {
            if (tileMapObject != null) {
                tileMapObjectContainer.AddChild(tileMapObject);
                tileMapObject.SetPosition(tileMapObject.WorldPosition());
            }
        }
    }
}
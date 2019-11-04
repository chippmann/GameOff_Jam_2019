using GameOff_2019.Levels.Common.TileMapObjects;
using Godot;

namespace GameOff_2019.Entities.Common.Navigation {
    public class DebugPathfinding : Node2D {
        [Export] private readonly NodePath pathfindingTileMapNodePath = null;
        private PathfindingTileMap pathfindingTileMap;
        [Export] private readonly NodePath tileMapManipulatorNodePath = null;
        private TileMapManipulator tileMapManipulator;

        private Vector2 lastTarget = Vector2.Zero;

        public override void _Ready() {
            pathfindingTileMap = GetNode<PathfindingTileMap>(pathfindingTileMapNodePath);
            tileMapManipulator = GetNode<TileMapManipulator>(tileMapManipulatorNodePath);
        }

        public override void _Input(InputEvent @event) {
            if (@event.IsActionPressed("debugPlantTree")) {
                var mousePosition = GetGlobalMousePosition();
                if (pathfindingTileMap.IsWorldPositionInTileMap(mousePosition)) {
                    if ((pathfindingTileMap.GetCell((int) pathfindingTileMap.WorldToMap(mousePosition).x, (int) pathfindingTileMap.WorldToMap(mousePosition).y) == pathfindingTileMap.traversableId
                         || pathfindingTileMap.GetCell((int) pathfindingTileMap.WorldToMap(mousePosition).x, (int) pathfindingTileMap.WorldToMap(mousePosition).y) == pathfindingTileMap.playerTraversableId)
                        && tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(pathfindingTileMap.WorldToMap(mousePosition)).CanInteract()) {
                        tileMapManipulator.SetupOrReplaceTileMapObject(pathfindingTileMap.WorldToMap(mousePosition), pathfindingTileMap.treeId);
                    }
                    else if (pathfindingTileMap.GetCell((int) pathfindingTileMap.WorldToMap(mousePosition).x, (int) pathfindingTileMap.WorldToMap(mousePosition).y) == pathfindingTileMap.treeId &&
                             tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(pathfindingTileMap.WorldToMap(mousePosition)).CanInteract()) {
                        tileMapManipulator.SetupOrReplaceTileMapObject(pathfindingTileMap.WorldToMap(mousePosition), pathfindingTileMap.traversableId);
                    }
                }
            }
        }
    }
}
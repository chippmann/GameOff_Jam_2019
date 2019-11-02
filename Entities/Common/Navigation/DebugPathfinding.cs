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
//            if (@event.IsActionPressed("debugPathfinding")) {
//                var mousePosition = GetGlobalMousePosition();
//                var path = pathfindingTileMap.FindPathToTarget(lastTarget, mousePosition);
//                lastTarget = mousePosition;
//            }
//
            if (@event.IsActionPressed("debugPlantTree")) {
                var mousePosition = GetGlobalMousePosition();
                if (pathfindingTileMap.IsWorldPositionInTileMap(mousePosition)) {
                    tileMapManipulator.MakeTileNotTraversable(mousePosition);
                }
            }
        }
    }
}
using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common;
using Godot;

namespace GameOff_2019.Entities.PlayerEntity {
    public class Player : EntityBody {
        [Export] private readonly NodePath navigationCheckerNodePath = null;
        private NavigationChecker navigationChecker;

        public override void _Ready() {
            base._Ready();
            navigationChecker = GetNode<NavigationChecker>(navigationCheckerNodePath);
        }

        public override void _Input(InputEvent @event) {
            if (@event.IsActionPressed("debugPathfinding")) {
                var mousePosition = GetGlobalMousePosition();
                var tileMapPosition = pathfindingTileMap.WorldToMap(mousePosition);
                if (navigationChecker.CanNavigateToPoint(pathfindingTileMap.tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(tileMapPosition))) {
                    entityMovement.MoveToPosition(TargetReached, mousePosition);
                }
                else {
                    Logger.Debug("Cannot navigate there! Is not inside an actionRadius");
                }
            }

//            if (@event.IsActionPressed("debugPlantTree")) {
//                var mousePosition = GetGlobalMousePosition();
//                if (pathfindingTileMap.IsWorldPositionInTileMap(mousePosition)) {
//                    tileMapManipulator.MakeTileNotTraversable(mousePosition);
//                }
//            }
        }

        private void TargetReached() { }
    }
}
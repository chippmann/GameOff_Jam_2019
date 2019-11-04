using GameOff_2019.Entities.Common;
using Godot;

namespace GameOff_2019.Entities.PlayerEntity {
    public class Player : EntityBody {
        public override void _Input(InputEvent @event) {
            if (@event.IsActionPressed("debugPathfinding")) {
                var mousePosition = GetGlobalMousePosition();
                entityMovement.MoveToPosition(TargetReached, mousePosition, true);
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
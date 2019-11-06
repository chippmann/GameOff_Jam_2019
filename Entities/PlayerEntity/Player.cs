using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common;
using Godot;

namespace GameOff_2019.Entities.PlayerEntity {
    public class Player : EntityBody {
        [Export] private readonly NodePath removeTreeCheckerNodePath = null;
        private RemoveTreeChecker removeTreeChecker;

        public override void _Ready() {
            base._Ready();
            removeTreeChecker = GetNode<RemoveTreeChecker>(removeTreeCheckerNodePath);
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.PlayerTargetReached), this, nameof(PlantTree));
        }

        public override void _Input(InputEvent @event) {
            if (@event.IsActionPressed("debugPathfinding")) {
                var mousePosition = GetGlobalMousePosition();
                entityMovement.MoveToPosition(mousePosition, new object[] { }, true);
            }

            if (@event.IsActionPressed("debugPlantTree")) {
                var mousePosition = GetGlobalMousePosition();
                if (pathfindingTileMap.IsWorldPositionInTileMap(mousePosition)) {
                    if (
                        (pathfindingTileMap.GetCell((int) pathfindingTileMap.WorldToMap(mousePosition).x, (int) pathfindingTileMap.WorldToMap(mousePosition).y) == pathfindingTileMap.traversableId
                         || pathfindingTileMap.GetCell((int) pathfindingTileMap.WorldToMap(mousePosition).x, (int) pathfindingTileMap.WorldToMap(mousePosition).y) == pathfindingTileMap.playerTraversableId)
                        && pathfindingTileMap.tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(pathfindingTileMap.WorldToMap(mousePosition)).CanInteract()
                    ) {
                        entityMovement.MoveToPosition(mousePosition, new object[] {mousePosition}, true, true);
                    }
                    else if (
                        pathfindingTileMap.GetCell((int) pathfindingTileMap.WorldToMap(mousePosition).x, (int) pathfindingTileMap.WorldToMap(mousePosition).y) == pathfindingTileMap.treeId
                        && pathfindingTileMap.tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(pathfindingTileMap.WorldToMap(mousePosition)).CanInteract()
                        && removeTreeChecker.CanRemoveTree(pathfindingTileMap.tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(pathfindingTileMap.WorldToMap(mousePosition)))
                    ) {
                        pathfindingTileMap.tileMapManipulator.DeleteTree(mousePosition);
                    }
                }
            }
        }

        private void PlantTree(Vector2 callbackParams) {
            pathfindingTileMap.tileMapManipulator.SetTree(callbackParams);
//            pathfindingTileMap.tileMapManipulator.SetupOrReplaceTileMapObject(pathfindingTileMap.WorldToMap(callbackParams), pathfindingTileMap.treeId);
        }
    }
}
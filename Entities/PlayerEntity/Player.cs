using GameOff_2019.Entities.Common;
using GameOff_2019.Entities.PlayerEntity.States.Message;
using GameOff_2019.Levels.Common.TileMapObjects.TreeObject;
using Godot;

namespace GameOff_2019.Entities.PlayerEntity {
    public class Player : EntityBody {
        [Export] private readonly NodePath removeTreeCheckerNodePath = null;
        private RemoveTreeChecker removeTreeChecker;
        [Export] private readonly NodePath playerStateMachineNodePath = null;
        private PlayerStateMachine playerStateMachine;

        public override void _Ready() {
            base._Ready();
            removeTreeChecker = GetNode<RemoveTreeChecker>(removeTreeCheckerNodePath);
            playerStateMachine = GetNode<PlayerStateMachine>(playerStateMachineNodePath);
//            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.PlayerTargetReached), this, nameof(PlantTree));
        }

        public override void _UnhandledInput(InputEvent @event) {
            if (@event.IsActionPressed("debugPathfinding")) {
                var mousePosition = GetGlobalMousePosition();
                playerStateMachine.TransitionTo(playerStateMachine.moveToPosition, new MoveToPositionMessage(mousePosition));
//                entityMovement.MoveToPosition(mousePosition, new object[] { }, true);
            }

            if (@event.IsActionPressed("debugPlantTree")) {
                var mousePosition = GetGlobalMousePosition();
                if (pathfindingTileMap.IsWorldPositionInTileMap(mousePosition)) {
                    if (
                        (pathfindingTileMap.GetCell((int) pathfindingTileMap.WorldToMap(mousePosition).x, (int) pathfindingTileMap.WorldToMap(mousePosition).y) == pathfindingTileMap.traversableId
                         || pathfindingTileMap.GetCell((int) pathfindingTileMap.WorldToMap(mousePosition).x, (int) pathfindingTileMap.WorldToMap(mousePosition).y) == pathfindingTileMap.playerTraversableId)
                        && pathfindingTileMap.tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(pathfindingTileMap.WorldToMap(mousePosition)).CanInteract()
                    ) {
                        playerStateMachine.TransitionTo(playerStateMachine.plantTree, new MoveToPositionMessage(mousePosition));
//                        entityMovement.MoveToPosition(mousePosition, new object[] {mousePosition}, true, true);
                    }
                    else if (
                        pathfindingTileMap.GetCell((int) pathfindingTileMap.WorldToMap(mousePosition).x, (int) pathfindingTileMap.WorldToMap(mousePosition).y) == pathfindingTileMap.treeId
                        && pathfindingTileMap.tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(pathfindingTileMap.WorldToMap(mousePosition)).CanInteract()
                    ) {
                        (pathfindingTileMap.tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(pathfindingTileMap.WorldToMap(mousePosition)) as TreeTileMapObject)?.Interact();
                    }
                }
            }

            base._UnhandledInput(@event);
        }

//        private void PlantTree(Vector2 callbackParams) {
//            pathfindingTileMap.tileMapManipulator.SetTree(callbackParams);
//        }

        public void HealTree(TreeTileMapObject tree) {
            playerStateMachine.TransitionTo(playerStateMachine.healTree, new MoveToPositionMessage(pathfindingTileMap.WorldToMap(tree.GetGlobalPosition())));
        }

        public bool CanRemoveTree(Vector2 treePosition) {
            return removeTreeChecker.CanRemoveTree(pathfindingTileMap.tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(pathfindingTileMap.WorldToMap(treePosition)));
        }
    }
}
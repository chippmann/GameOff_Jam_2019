using GameOff_2019.EngineUtils;
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
        }

        public override void _UnhandledInput(InputEvent @event) {
            if (@event.IsActionPressed("debugPathfinding")) {
                var mousePosition = GetGlobalMousePosition();
                playerStateMachine.TransitionTo(playerStateMachine.moveToPosition, new MoveToPositionMessage(mousePosition));
                GetTree().SetInputAsHandled();
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
                        GetTree().SetInputAsHandled();
                    }
                    else if (
                        pathfindingTileMap.GetCell((int) pathfindingTileMap.WorldToMap(mousePosition).x, (int) pathfindingTileMap.WorldToMap(mousePosition).y) == pathfindingTileMap.treeId
                        && pathfindingTileMap.tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(pathfindingTileMap.WorldToMap(mousePosition)).CanInteract()
                    ) {
                        (pathfindingTileMap.tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(pathfindingTileMap.WorldToMap(mousePosition)) as TreeTileMapObject)?.Interact();
                        GetTree().SetInputAsHandled();
                    }
                }
            }

            if (@event.IsActionPressed(GameConstants.ControlsActionCancel)) {
                playerStateMachine.TransitionTo(playerStateMachine.idle);
                GetTree().SetInputAsHandled();
            }

            base._UnhandledInput(@event);
        }

        public void HealTree(TreeTileMapObject tree) {
            playerStateMachine.TransitionTo(playerStateMachine.healTree, new MoveToPositionMessage(pathfindingTileMap.WorldToMap(tree.GetGlobalPosition())));
        }

        public bool CanRemoveTree(Vector2 treeWorldPosition) {
            return removeTreeChecker.CanRemoveTree(pathfindingTileMap.tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(pathfindingTileMap.WorldToMap(treeWorldPosition)));
        }
    }
}
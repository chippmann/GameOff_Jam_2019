using Godot;
using Planty.EngineUtils;
using Planty.Entities.Common;
using Planty.Entities.PlayerEntity.States.Message;
using Planty.Levels.Common.TileMapObjects.TreeObject;
using Planty.RoundLogic;

namespace Planty.Entities.PlayerEntity {
    public class Player : Entity {
        [Export] private readonly NodePath removeTreeCheckerNodePath = null;
        private RemoveTreeChecker removeTreeChecker;
        [Export] private readonly NodePath playerStateMachineNodePath = null;
        private PlayerStateMachine playerStateMachine;

        private GameState gameState;

        public override void _Ready() {
            base._Ready();
            removeTreeChecker = GetNode<RemoveTreeChecker>(removeTreeCheckerNodePath);
            playerStateMachine = GetNode<PlayerStateMachine>(playerStateMachineNodePath);
            gameState = NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true);
        }

        public override void _UnhandledInput(InputEvent @event) {
            if (!gameState.isTileMapSetup) {
                return;
            }

            if (@event.IsActionPressed("debugPathfinding")) {
                var mousePosition = GetGlobalMousePosition();
                playerStateMachine.TransitionTo(playerStateMachine.moveToPosition, new MoveToPositionMessage(mousePosition));
                GetTree().SetInputAsHandled();
            }

            if (@event.IsActionPressed("debugPlantTree")) {
                var mousePosition = GetGlobalMousePosition();
                if (pathfindingTileMap.IsWorldPositionInTileMap(mousePosition)) {
                    if (
                        gameState.GetPlayerEnergy() >= GameValues.plantTreeEnergyUsage
                        && (pathfindingTileMap.GetCell((int) pathfindingTileMap.WorldToMap(mousePosition).x, (int) pathfindingTileMap.WorldToMap(mousePosition).y) == pathfindingTileMap.traversableId
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

        public bool CanHealTree() {
            return gameState.GetPlayerEnergy() >= GameValues.healTreeEnergyUsage;
        }
    }
}
using System;
using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common;
using GameOff_2019.Entities.Common.Movement;
using GameOff_2019.Entities.Common.Navigation;
using GameOff_2019.Entities.Common.StateMachine;
using GameOff_2019.Entities.PlayerEntity.States.Message;
using GameOff_2019.RoundLogic;
using Godot;

namespace GameOff_2019.Entities.PlayerEntity.States {
    public class PlayerPlantTreeState : State {
        [Export] private readonly NodePath entityMovementNodePath = null;
        private EntityMovement entityMovement;
        private PathfindingTileMap pathfindingTileMap;
        private Vector2 targetPosition;

        private GameState gameState;

        public override void _Ready() {
            base._Ready();
            entityMovement = GetNode<EntityMovement>(entityMovementNodePath);
            gameState = NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true);

            pathfindingTileMap = NodeGetter.GetFirstNodeInGroup<PathfindingTileMap>(GetTree(), GameConstants.PathfindingTileMapGroup, true);
        }

        public override void Enter(IStateMachineMessage message = null) {
            if (!(message is MoveToPositionMessage)) {
                throw new Exception("State message is not of Type \"MoveToPositionMessage\"");
            }

            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.PlayerTargetReached), this, nameof(TargetReached));
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.TargetCannotBeReached), this, nameof(PlayerTargetCannotBeReached));
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.InvalidatePlayerPath), this, nameof(OnPathInvalidated));
            targetPosition = ((MoveToPositionMessage) message).GetTargetPosition();
            OnPathInvalidated();
        }

        public override void UnhandledInput(InputEvent @event) { }

        public override void PhysicsProcess(float delta) { }

        public override void Exit() {
            entityMovement.StopMovement();
            if (GetNode<Eventing>(Eventing.EventingNodePath).IsConnected(nameof(Eventing.InvalidatePlayerPath), this, nameof(OnPathInvalidated))) {
                GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.InvalidatePlayerPath), this, nameof(OnPathInvalidated));
            }

            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.PlayerTargetReached), this, nameof(TargetReached));
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.TargetCannotBeReached), this, nameof(PlayerTargetCannotBeReached));
        }

        public override string GetName() {
            return "PlayerPlantTreeState";
        }

        private void TargetReached() {
            if (GetNode<Eventing>(Eventing.EventingNodePath).IsConnected(nameof(Eventing.InvalidatePlayerPath), this, nameof(OnPathInvalidated))) {
                GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.InvalidatePlayerPath), this, nameof(OnPathInvalidated));
            }

            gameState.UsePlayerEnergy(GameValues.plantTreeEnergyUsage);
            pathfindingTileMap.tileMapManipulator.SetTree(targetPosition);
            gameState.AddPlayerPoints(GameValues.plantTreePoints);
            GetStateMachine<PlayerStateMachine>().TransitionTo(GetStateMachine<PlayerStateMachine>().idle);
        }

        private void PlayerTargetCannotBeReached(Entity sourceEntity) {
            if (sourceEntity is Player) {
                GetStateMachine<PlayerStateMachine>().TransitionTo(GetStateMachine<PlayerStateMachine>().idle);
            }
        }

        private void OnPathInvalidated() {
            entityMovement.MoveToPosition(targetPosition, isPlayer: true, minusOne: true, paramsToReturn: new object[] { }, targetCannotBeReachedParamsToReturn: new object[] {GetOwner<Player>()});
        }
    }
}
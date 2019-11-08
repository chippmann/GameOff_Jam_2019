using System;
using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common.Movement;
using GameOff_2019.Entities.Common.StateMachine;
using GameOff_2019.Entities.PlayerEntity.States.Message;
using Godot;

namespace GameOff_2019.Entities.PlayerEntity.States {
    public class PlayerMoveToPositionState : State {
        [Export] private readonly NodePath entityMovementNodePath = null;
        private EntityMovement entityMovement;
        private Vector2 targetPosition;

        public override void _Ready() {
            base._Ready();
            entityMovement = GetNode<EntityMovement>(entityMovementNodePath);
        }

        public override void Enter(IStateMachineMessage message = null) {
            if (!(message is MoveToPositionMessage)) {
                throw new Exception("State message is not of Type \"MoveToPositionMessage\"");
            }

            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.PlayerTargetReached), this, nameof(TargetReached));
            targetPosition = ((MoveToPositionMessage) message).GetTargetPosition();
            entityMovement.MoveToPosition(targetPosition, isPlayer: true, paramsToReturn: new object[] { });
        }

        public override void UnhandledInput(InputEvent @event) { }

        public override void PhysicsProcess(float delta) { }

        public override void Exit() {
            entityMovement.StopMovement();
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.PlayerTargetReached), this, nameof(TargetReached));
        }

        public override string GetName() {
            return "PlayerMoveToPositionState";
        }

        private void TargetReached() {
            GetStateMachine<PlayerStateMachine>().TransitionTo(GetStateMachine<PlayerStateMachine>().idle);
        }
    }
}
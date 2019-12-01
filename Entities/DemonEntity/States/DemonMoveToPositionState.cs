using System;
using Godot;
using Planty.EngineUtils;
using Planty.Entities.Common;
using Planty.Entities.Common.Movement;
using Planty.Entities.Common.StateMachine;
using Planty.Entities.PlayerEntity.States.Message;
using Planty.SoundEngine;

namespace Planty.Entities.DemonEntity.States {
    public class DemonMoveToPositionState : State {
        [Export] private readonly NodePath entityMovementNodePath = null;
        [Export] private AudioStreamOGGVorbis deamonWalkSound = null;
        private EntityMovement entityMovement;
        private Vector2 targetPosition;
        private SoundEngineNode soundEngineNode;
        private AudioStreamPlayer2D soundPlayer;

        public override void _Ready() {
            base._Ready();
            entityMovement = GetNode<EntityMovement>(entityMovementNodePath);
            soundEngineNode = NodeGetter.GetFirstNodeInGroup<SoundEngineNode>(GetTree(), GameConstants.SoundEngineGroup, true);
        }

        public override void Enter(IStateMachineMessage message = null) {
            if (!(message is MoveToPositionMessage)) {
                throw new Exception("State message is not of Type \"MoveToPositionMessage\"");
            }

            soundPlayer = soundEngineNode.PlaySfxLoop2D(deamonWalkSound, NodeGetter.GetFirstNodeInGroup<Demon>(GetTree(), GameConstants.DemonGroup, true));

            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.DemonTargetReached), this, nameof(TargetReached));
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.TargetCannotBeReached), this, nameof(TargetCannotBeReached));
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.InvalidateDemonPath), this, nameof(OnPathInvalidated));
            targetPosition = ((MoveToPositionMessage) message).GetTargetPosition();
            OnPathInvalidated();
        }

        public override void UnhandledInput(InputEvent @event) { }

        public override void PhysicsProcess(float delta) { }

        public override void Exit() {
            entityMovement.StopMovement();

            soundEngineNode.StopSfx(soundPlayer);

            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.DemonTargetReached), this, nameof(TargetReached));
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.TargetCannotBeReached), this, nameof(TargetCannotBeReached));
            if (GetNode<Eventing>(Eventing.EventingNodePath).IsConnected(nameof(Eventing.InvalidateDemonPath), this, nameof(OnPathInvalidated))) {
                GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.InvalidateDemonPath), this, nameof(OnPathInvalidated));
            }
        }

        public override string GetName() {
            return "DemonMoveToPositionState";
        }

        private void TargetReached() {
            if (GetNode<Eventing>(Eventing.EventingNodePath).IsConnected(nameof(Eventing.InvalidateDemonPath), this, nameof(OnPathInvalidated))) {
                GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.InvalidateDemonPath), this, nameof(OnPathInvalidated));
            }

            GetStateMachine<DemonStateMachine>().TransitionTo(GetStateMachine<DemonStateMachine>().idle);
        }

        private void TargetCannotBeReached(Entity sourceEntity) {
            if (sourceEntity is Demon) {
                GetStateMachine<DemonStateMachine>().TransitionTo(GetStateMachine<DemonStateMachine>().idle);
            }
        }

        private void OnPathInvalidated() {
            entityMovement.MoveToPosition(targetPosition, isPlayer: false, paramsToReturn: new object[] { }, targetCannotBeReachedParamsToReturn: new object[] {GetOwner<Demon>()});
        }
    }
}
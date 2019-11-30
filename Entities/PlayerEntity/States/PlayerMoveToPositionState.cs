using System;
using System.Threading.Tasks;
using Godot;
using Planty.EngineUtils;
using Planty.Entities.Common;
using Planty.Entities.Common.Movement;
using Planty.Entities.Common.StateMachine;
using Planty.Entities.PlayerEntity.States.Message;
using Planty.SoundEngine;

namespace Planty.Entities.PlayerEntity.States {
    public class PlayerMoveToPositionState : State {
        [Export] private readonly NodePath entityMovementNodePath = null;
        [Export] private AudioStreamOGGVorbis playerWalkSound = null;
        private EntityMovement entityMovement;
        private Vector2 targetPosition;
        private SoundEngineNode soundEngineNode;
        private AudioStreamPlayer soundPlayer;

        public override void _Ready() {
            base._Ready();
            entityMovement = GetNode<EntityMovement>(entityMovementNodePath);
            soundEngineNode = NodeGetter.GetFirstNodeInGroup<SoundEngineNode>(GetTree(), GameConstants.SoundEngineGroup, true);
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
            soundPlayer = soundEngineNode.PlaySfxLoop(playerWalkSound, this);
        }

        public override void UnhandledInput(InputEvent @event) { }

        public override void PhysicsProcess(float delta) { }

        public override void Exit() {
            entityMovement.StopMovement();

            soundEngineNode.StopSfx(soundPlayer);

            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.PlayerTargetReached), this, nameof(TargetReached));
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.TargetCannotBeReached), this, nameof(PlayerTargetCannotBeReached));
            if (GetNode<Eventing>(Eventing.EventingNodePath).IsConnected(nameof(Eventing.InvalidatePlayerPath), this, nameof(OnPathInvalidated))) {
                GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.InvalidatePlayerPath), this, nameof(OnPathInvalidated));
            }
        }

        public override string GetName() {
            return "PlayerMoveToPositionState";
        }

        private void TargetReached() {
            if (GetNode<Eventing>(Eventing.EventingNodePath).IsConnected(nameof(Eventing.InvalidatePlayerPath), this, nameof(OnPathInvalidated))) {
                GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.InvalidatePlayerPath), this, nameof(OnPathInvalidated));
            }

            GetStateMachine<PlayerStateMachine>().TransitionTo(GetStateMachine<PlayerStateMachine>().idle);
        }

        private void PlayerTargetCannotBeReached(Entity sourceEntity) {
            if (sourceEntity is Player) {
                GetStateMachine<PlayerStateMachine>().TransitionTo(GetStateMachine<PlayerStateMachine>().idle);
            }
        }

        private void OnPathInvalidated() {
            entityMovement.MoveToPosition(targetPosition, isPlayer: true, paramsToReturn: new object[] { }, targetCannotBeReachedParamsToReturn: new object[] {GetOwner<Player>()});
        }
    }
}
using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common.BehaviourTree;
using GameOff_2019.Entities.PlayerEntity.States.Message;
using Godot;

namespace GameOff_2019.Entities.DemonEntity.Behaviours {
    public abstract class BaseDemonWanderBehaviour : BTAction {
        [Export] private readonly NodePath stateMachineNodePath = null;
        private DemonStateMachine stateMachine;
        [Export] private readonly NodePath movePositionFinderNodePath = null;
        protected MovePositionFinder movePositionFinder;

        private bool running = false;
        private BTResult btResult = BTResult.Failure;

        public override void _Ready() {
            base._Ready();
            stateMachine = GetNode<DemonStateMachine>(stateMachineNodePath);
            movePositionFinder = GetNode<MovePositionFinder>(movePositionFinderNodePath);
        }

        protected override BTResult ExecuteInternal() {
            if (!running && btResult != BTResult.Running) {
                running = true;
                btResult = BTResult.Running;
                GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.DemonTargetReached), this, nameof(TargetReached));
                GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.TargetCannotBeReached), this, nameof(TargetCannotBeReached));
                stateMachine.TransitionTo(stateMachine.moveToPosition, new MoveToPositionMessage(GetWanderWorldPosition()));
            }

            return btResult;
        }

        protected abstract Vector2 GetWanderWorldPosition();

        private void TargetReached() {
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.DemonTargetReached), this, nameof(TargetReached));
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.TargetCannotBeReached), this, nameof(TargetCannotBeReached));
            btResult = BTResult.Success;
            running = false;
        }

        private void TargetCannotBeReached() {
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.DemonTargetReached), this, nameof(TargetReached));
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.TargetCannotBeReached), this, nameof(TargetCannotBeReached));
            btResult = BTResult.Failure;
            running = false;
        }
    }
}
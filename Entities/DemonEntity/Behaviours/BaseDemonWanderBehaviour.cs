using Godot;
using Planty.EngineUtils;
using Planty.Entities.Common;
using Planty.Entities.Common.BehaviourTree;
using Planty.Entities.PlayerEntity.States.Message;

namespace Planty.Entities.DemonEntity.Behaviours {
    public abstract class BaseDemonWanderBehaviour : BTAction {
        [Export] private readonly NodePath stateMachineNodePath = null;
        private DemonStateMachine stateMachine;
        [Export] private readonly NodePath movePositionFinderNodePath = null;
        protected MovePositionFinder movePositionFinder;

        private bool finished = false;
        private bool running = false;
        private BTResult btResult = BTResult.Failure;

        public override void _Ready() {
            base._Ready();
            stateMachine = GetNode<DemonStateMachine>(stateMachineNodePath);
            movePositionFinder = GetNode<MovePositionFinder>(movePositionFinderNodePath);
        }

        protected override BTResult ExecuteInternal() {
            if (!running) {
                finished = false;
                running = true;
                btResult = BTResult.Running;
                GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.DemonTargetReached), this, nameof(TargetReached));
                GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.TargetCannotBeReached), this, nameof(TargetCannotBeReached));
                stateMachine.TransitionTo(stateMachine.moveToPosition, new MoveToPositionMessage(GetWanderWorldPosition()));
            }

            if (finished && running) {
                running = false;
                return btResult;
            }

            return btResult;
        }

        protected abstract Vector2 GetWanderWorldPosition();

        private void TargetReached() {
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.DemonTargetReached), this, nameof(TargetReached));
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.TargetCannotBeReached), this, nameof(TargetCannotBeReached));
            btResult = BTResult.Success;
            finished = true;
        }

        private void TargetCannotBeReached(Entity ignored) {
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.DemonTargetReached), this, nameof(TargetReached));
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.TargetCannotBeReached), this, nameof(TargetCannotBeReached));
            btResult = BTResult.Failure;
            finished = true;
        }
    }
}
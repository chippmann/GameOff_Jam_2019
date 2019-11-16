using System;
using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common.BehaviourTree;
using GameOff_2019.Entities.PlayerEntity.States.Message;
using Godot;

namespace GameOff_2019.Entities.DemonEntity.Behaviours {
    public abstract class BaseInfestTreeBehaviour : BTAction {
        [Export] private readonly NodePath stateMachineNodePath = null;
        private DemonStateMachine stateMachine;
        [Export] private readonly NodePath movePositionFinderNodePath = null;
        protected MovePositionFinder movePositionFinder;
        [Export] public readonly int energyConsumption = 20;
        [Export] private readonly Vector2 randomTimeRangeBetweenTreeInfestions = new Vector2(10, 30);
        [Export] private readonly NodePath timerNodePath = null;
        public Timer timer;

        private bool running = false;
        private BTResult btResult = BTResult.Failure;

        public override void _Ready() {
            base._Ready();
            stateMachine = GetNode<DemonStateMachine>(stateMachineNodePath);
            movePositionFinder = GetNode<MovePositionFinder>(movePositionFinderNodePath);
            timer = GetNode<Timer>(timerNodePath);
            timer.SetOneShot(true);
        }

        protected override BTResult ExecuteInternal() {
            if (!running && btResult != BTResult.Running) {
                running = true;
                btResult = BTResult.Running;
                GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.TreeInfested), this, nameof(TreeInfested));
                GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.TargetCannotBeReached), this, nameof(TargetCannotBeReached));
                stateMachine.TransitionTo(stateMachine.moveToPosition, new MoveToPositionMessage(GetTreeToInfestWorldPosition()));
            }

            return btResult;
        }

        protected abstract Vector2 GetTreeToInfestWorldPosition();

        private void TreeInfested() {
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.TreeInfested), this, nameof(TreeInfested));
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.TargetCannotBeReached), this, nameof(TargetCannotBeReached));
            btResult = BTResult.Success;
            var random = new Random();
            var randomTime = random.Next((int) randomTimeRangeBetweenTreeInfestions.x, (int) randomTimeRangeBetweenTreeInfestions.y);
            timer.Start(randomTime);
        }

        private void TargetCannotBeReached() {
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.TreeInfested), this, nameof(TreeInfested));
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.TargetCannotBeReached), this, nameof(TargetCannotBeReached));
            btResult = BTResult.Failure;
        }
    }
}
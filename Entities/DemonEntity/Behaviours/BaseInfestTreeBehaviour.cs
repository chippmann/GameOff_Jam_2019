using System;
using Godot;
using Planty.EngineUtils;
using Planty.Entities.Common.BehaviourTree;
using Planty.Entities.PlayerEntity.States.Message;
using Planty.Levels.Common.TileMapObjects.TreeObject;

namespace Planty.Entities.DemonEntity.Behaviours {
    public abstract class BaseInfestTreeBehaviour : BTAction {
        [Export] private readonly NodePath stateMachineNodePath = null;
        private DemonStateMachine stateMachine;
        [Export] private readonly NodePath movePositionFinderNodePath = null;
        protected MovePositionFinder movePositionFinder;
        [Export] public readonly int energyConsumption = 20;
        [Export] private readonly Vector2 randomTimeRangeBetweenTreeInfestions = new Vector2(5, 20);
        [Export] private readonly NodePath timerNodePath = null;
        public Timer timer;

        private bool finished = false;
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
            if (!running) {
                finished = false;
                running = true;
                btResult = BTResult.Running;
                GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.TreeInfested), this, nameof(TreeInfested));
                GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.TargetCannotBeReached), this, nameof(TargetCannotBeReached));
                var treeToInfestWorldPosition = GetTreeToInfestWorldPosition();
                if (treeToInfestWorldPosition == new Vector2(-1, -1)) {
                    stateMachine.TransitionTo(stateMachine.idle);
                    TargetCannotBeReached();
                }
                else {
                    stateMachine.TransitionTo(stateMachine.infestTree, new MoveToPositionMessage(treeToInfestWorldPosition));
                }
            }

            if (finished && running) {
                running = false;
                return btResult;
            }

            return btResult;
        }

        protected abstract Vector2 GetTreeToInfestWorldPosition();

        private void TreeInfested(TreeTileMapObject infestedTree) {
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.TreeInfested), this, nameof(TreeInfested));
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.TargetCannotBeReached), this, nameof(TargetCannotBeReached));
            btResult = BTResult.Success;
            var random = new Random();
            var randomTime = random.Next((int) randomTimeRangeBetweenTreeInfestions.x, (int) randomTimeRangeBetweenTreeInfestions.y);
            timer.Start(randomTime);
            finished = true;
        }

        private void TargetCannotBeReached() {
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.TreeInfested), this, nameof(TreeInfested));
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.TargetCannotBeReached), this, nameof(TargetCannotBeReached));
            btResult = BTResult.Failure;
            finished = true;
        }
    }
}
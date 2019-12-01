using Godot;
using Planty.EngineUtils;
using Planty.Entities.Common.BehaviourTree;
using Planty.Entities.DemonEntity.States;

namespace Planty.Entities.DemonEntity.Behaviours {
    public class DemonIdleBehaviour : BTAction {
        [Export] private readonly NodePath stateMachineNodePath = null;
        private DemonStateMachine stateMachine;

        public override void _Ready() {
            base._Ready();
            stateMachine = GetNode<DemonStateMachine>(stateMachineNodePath);
        }

        protected override BTResult ExecuteInternal() {
            if (!(stateMachine.GetCurrentState() is DemonIdleState)) {
                Logger.Debug("Demon is going to idle");
                stateMachine.TransitionTo(stateMachine.idle);
            }

            return BTResult.Success;
        }
    }
}
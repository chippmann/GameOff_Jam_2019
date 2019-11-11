using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common.BehaviourTree;
using GameOff_2019.Entities.DemonEntity.States;
using Godot;

namespace GameOff_2019.Entities.DemonEntity.Behaviours {
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
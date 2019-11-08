using GameOff_2019.Entities.Common.StateMachine;
using Godot;

namespace GameOff_2019.Entities.PlayerEntity.States {
    public class PlayerIdleState : State {
        public override void Enter(IStateMachineMessage message = null) { }

        public override void UnhandledInput(InputEvent @event) { }

        public override void PhysicsProcess(float delta) { }

        public override void Exit() { }

        public override string GetName() {
            return "PlayerIdleState";
        }
    }
}
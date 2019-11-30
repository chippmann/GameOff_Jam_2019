using Godot;

namespace Planty.Entities.Common.StateMachine {
    public abstract class State : Node2D {
        private FiniteStateMachine stateMachine;
        private State parentState;

        public override void _Ready() {
            stateMachine = AssignStateMachine(this);
            if (GetParent() is State) {
                parentState = GetParent<State>();
            }
        }

        private static FiniteStateMachine AssignStateMachine(Node node) {
            if (node != null && !node.IsInGroup(FiniteStateMachine.StateMachineGroup)) {
                return AssignStateMachine(node.GetParent());
            }

            return node as FiniteStateMachine;
        }


        public abstract void Enter(IStateMachineMessage message = null);
        public abstract void UnhandledInput(InputEvent @event);
        public abstract void PhysicsProcess(float delta);
        public abstract void Exit();
        public new abstract string GetName();

        public T GetStateMachine<T>() where T : FiniteStateMachine {
            return stateMachine as T;
        }
    }
}
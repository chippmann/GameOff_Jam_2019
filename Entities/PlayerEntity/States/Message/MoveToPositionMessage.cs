using Godot;
using Planty.Entities.Common.StateMachine;

namespace Planty.Entities.PlayerEntity.States.Message {
    public class MoveToPositionMessage : IStateMachineMessage {
        private readonly Vector2 targetPosition;

        public MoveToPositionMessage(Vector2 targetPosition) {
            this.targetPosition = targetPosition;
        }

        public Vector2 GetTargetPosition() {
            return targetPosition;
        }
    }
}
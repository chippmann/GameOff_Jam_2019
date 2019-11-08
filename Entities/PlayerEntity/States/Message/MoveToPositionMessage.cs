using GameOff_2019.Entities.Common.StateMachine;
using Godot;

namespace GameOff_2019.Entities.PlayerEntity.States.Message {
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
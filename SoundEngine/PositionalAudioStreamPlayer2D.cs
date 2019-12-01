using Godot;
using Planty.EngineUtils;

namespace Planty.SoundEngine {
    public class PositionalAudioStreamPlayer2D : AudioStreamPlayer2D {
        private Node2D targetToFollow;

        public void Init(Node2D target) {
            this.targetToFollow = target;
        }

        public override void _Process(float delta) {
            base._Process(delta);
            if (targetToFollow == null) {
                Logger.Warning("Call init first.");
                return;
            }

            if (!IsInstanceValid(targetToFollow)) {
                SetGlobalPosition(Vector2.Zero);
                return;
            }

            SetGlobalPosition(targetToFollow.GetGlobalPosition());
        }
    }
}
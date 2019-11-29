using GameOff_2019.EngineUtils;
using Godot;

namespace GameOff_2019.SoundEngine {
    public class PositionalAudioStreamPlayer2D: AudioStreamPlayer2D {
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
            SetGlobalPosition(targetToFollow.GetGlobalPosition());
        }
    }
}
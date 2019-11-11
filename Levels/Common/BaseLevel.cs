using GameOff_2019.EngineUtils;
using Godot;

namespace GameOff_2019.Levels.Common {
    public class BaseLevel : Node2D {
        public override void _Ready() {
            base._Ready();
            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.LevelSetupFinished));
        }
    }
}
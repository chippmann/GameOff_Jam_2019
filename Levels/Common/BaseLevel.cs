using GameOff_2019.EngineUtils;
using GameOff_2019.RoundLogic;
using Godot;

namespace GameOff_2019.Levels.Common {
    public class BaseLevel : Node2D {
        [Export] private readonly NodePath gameUiNodePath = null;
        private Control gameUi;
        
        public override void _Ready() {
            base._Ready();
            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.LevelSetupFinished));
            gameUi = GetNode<Control>(gameUiNodePath);
        }

        public void SetLevelVisibility(bool isVisible) {
            SetVisible(isVisible);
            gameUi.SetVisible(isVisible);
        }
    }
}
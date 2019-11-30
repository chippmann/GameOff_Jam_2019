using GameOff_2019.EngineUtils;
using Godot;

namespace GameOff_2019.Ui.Menu.PauseMenu {
    public class PauseMenuManager : Control {
        [Export] private readonly NodePath continueButtonNodePath = null;
        private Button continueButton;
        [Export] private readonly NodePath finishGameButtonNodePath = null;
        private Button finishGameButton;

        public override void _Ready() {
            base._Ready();
            continueButton = GetNode<Button>(continueButtonNodePath);
            finishGameButton = GetNode<Button>(finishGameButtonNodePath);

            continueButton.Connect("pressed", this, nameof(OnContinueButtonPressed));
            finishGameButton.Connect("pressed", this, nameof(OnFinishGameButtonPressed));
        }

        private void OnContinueButtonPressed() {
            QueueFree();
        }

        private void OnFinishGameButtonPressed() {
            QueueFree();
            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.FinishCurrentGame));
        }
    }
}
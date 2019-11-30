using GameOff_2019.EngineUtils;
using GameOff_2019.RoundLogic;
using Godot;

namespace GameOff_2019.Ui.Menu.EndMenu {
    public class EndMenuManager : Control {
        [Export] private readonly string pointsLimitTitleText = "";
        [Export] private readonly string pointsLimitMessageText = "";
        [Export] private readonly string worldLimitTitleText = "";
        [Export] private readonly string worldLimitMessageText = "";
        [Export] private readonly NodePath titleLabelNodePath = null;
        private Label titleLabel;
        [Export] private readonly NodePath messageLabelNodePath = null;
        private Label messageLabel;
        [Export] private readonly NodePath continueButtonNodePath = null;
        private Button continueButton;

        public override void _Ready() {
            base._Ready();
            titleLabel = GetNode<Label>(titleLabelNodePath);
            messageLabel = GetNode<Label>(messageLabelNodePath);
            continueButton = GetNode<Button>(continueButtonNodePath);
            continueButton.Connect("pressed", this, nameof(OnContinueButtonPressed));

            var gameState = NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true);
            if (gameState.isPointLimitEnabled) {
                titleLabel.SetText(Tr(pointsLimitTitleText));
                messageLabel.SetText(Tr(pointsLimitMessageText));
            }
            else {
                titleLabel.SetText(Tr(worldLimitTitleText));
                messageLabel.SetText(Tr(worldLimitMessageText));
            }
        }

        private void OnContinueButtonPressed() {
            continueButton.SetDisabled(true);
            QueueFree();
            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.ContinuePressed));
        }
    }
}
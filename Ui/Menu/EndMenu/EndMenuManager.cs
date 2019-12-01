using Godot;
using Planty.EngineUtils;
using Planty.RoundLogic;
using Planty.SoundEngine;

namespace Planty.Ui.Menu.EndMenu {
    public class EndMenuManager : Control {
        [Export] private readonly string pointsLimitTitleText = "";
        [Export] private readonly string pointsLimitMessageText = "";
        [Export] private readonly string worldLimitTitleText = "";
        [Export] private readonly string worldLimitMessageText = "";
        [Export] private readonly NodePath titleLabelNodePath = null;
        [Export] private readonly AudioStreamOGGVorbis loosingSound = null;
        [Export] private readonly AudioStreamOGGVorbis winningSound = null;
        private Label titleLabel;
        [Export] private readonly NodePath messageLabelNodePath = null;
        private Label messageLabel;
        [Export] private readonly NodePath continueButtonNodePath = null;
        private Button continueButton;
        private SoundEngineNode soundEngineNode;

        public override void _Ready() {
            base._Ready();
            titleLabel = GetNode<Label>(titleLabelNodePath);
            messageLabel = GetNode<Label>(messageLabelNodePath);
            continueButton = GetNode<Button>(continueButtonNodePath);
            continueButton.Connect("pressed", this, nameof(OnContinueButtonPressed));

            soundEngineNode = NodeGetter.GetFirstNodeInGroup<SoundEngineNode>(GetTree(), GameConstants.SoundEngineGroup, true);
            if (pointsLimitTitleText.EndsWith("won")) {
                soundEngineNode.PlaySfx(winningSound, this);
            }
            else {   
                soundEngineNode.PlaySfx(loosingSound, this);
            }

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
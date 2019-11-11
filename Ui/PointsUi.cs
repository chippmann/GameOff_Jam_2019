using GameOff_2019.EngineUtils;
using GameOff_2019.RoundLogic;
using Godot;

namespace GameOff_2019.Ui {
    public class PointsUi : HBoxContainer {
        [Export] private readonly NodePath playerPointsLabelNodePath = null;
        private Label playerPointsLabel;
        [Export] private readonly NodePath demonPointsLabelNodePath = null;
        private Label demonPointsLabel;
        [Export] private readonly NodePath progressNodePath = null;
        private ProgressBar progress;
        [Export] private readonly NodePath progressTweenNodePath = null;
        private Tween progressTween;

        private GameState gameState;

        public override void _Ready() {
            base._Ready();
            playerPointsLabel = GetNode<Label>(playerPointsLabelNodePath);
            demonPointsLabel = GetNode<Label>(demonPointsLabelNodePath);
            progress = GetNode<ProgressBar>(progressNodePath);
            progressTween = GetNode<Tween>(progressTweenNodePath);
            gameState = NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true);

            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.PointsChanged), this, nameof(OnPointsChanged));
        }

        private void OnPointsChanged() {
            playerPointsLabel.SetText(Tr("playerPointsKey") + "\n" + gameState.GetPlayerPoints());
            demonPointsLabel.SetText(Tr("demonPointsKey") + "\n" + gameState.GetDemonPoints());
            progressTween.InterpolateMethod(progress, "set_value", progress.GetValue(), GetPointsPercentageValue() * 100f, 1f, Tween.TransitionType.Sine, Tween.EaseType.InOut);
            progressTween.Start();
        }

        private float GetPointsPercentageValue() {
            return gameState.GetPlayerPoints() / (float) (gameState.GetPlayerPoints() + gameState.GetDemonPoints());
        }
    }
}
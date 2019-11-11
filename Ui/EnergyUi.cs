using GameOff_2019.EngineUtils;
using GameOff_2019.RoundLogic;
using Godot;

namespace GameOff_2019.Ui {
    public class EnergyUi : HBoxContainer {
        [Export] private readonly NodePath playerEnergyProgressNodePath = null;
        private ProgressBar playerEnergyProgress;
        [Export] private readonly NodePath demonEnergyProgressNodePath = null;
        private ProgressBar demonEnergyProgress;
        [Export] private readonly NodePath playerEnergyTweenNodePath = null;
        private Tween playerEnergyTween;
        [Export] private readonly NodePath demonEnergyTweenNodePath = null;
        private Tween demonEnergyTween;

        private GameState gameState;

        public override void _Ready() {
            base._Ready();
            playerEnergyProgress = GetNode<ProgressBar>(playerEnergyProgressNodePath);
            demonEnergyProgress = GetNode<ProgressBar>(demonEnergyProgressNodePath);
            playerEnergyTween = GetNode<Tween>(playerEnergyTweenNodePath);
            demonEnergyTween = GetNode<Tween>(demonEnergyTweenNodePath);
            gameState = NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true);

            playerEnergyProgress.SetValue(gameState.GetPlayerEnergy());
            demonEnergyProgress.SetValue(gameState.GetDemonEnergy());

            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.PlayerEnergyChanged), this, nameof(OnPlayerEnergyChanged));
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.DemonEnergyChanged), this, nameof(OnDemonEnergyChanged));
        }

        private void OnPlayerEnergyChanged() {
            playerEnergyTween.InterpolateMethod(playerEnergyProgress, "set_value", playerEnergyProgress.GetValue(), gameState.GetPlayerEnergy(), 1f, Tween.TransitionType.Sine, Tween.EaseType.InOut);
            playerEnergyTween.Start();
        }

        private void OnDemonEnergyChanged() {
            demonEnergyTween.InterpolateMethod(demonEnergyProgress, "set_value", demonEnergyProgress.GetValue(), gameState.GetDemonEnergy(), 1f, Tween.TransitionType.Sine, Tween.EaseType.InOut);
            demonEnergyTween.Start();
        }
    }
}
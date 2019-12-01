using Godot;
using Planty.EngineUtils;
using Planty.Entities.Common.StateMachine;
using Planty.Entities.PlayerEntity;
using Planty.Levels.Common.TileMapObjects.TreeObject.Effects;
using Planty.RoundLogic;

namespace Planty.Levels.Common.TileMapObjects.TreeObject.TreeStates {
    public class FullyGrownState : State {
        [Export] private readonly int addPointsTimeIntervalInSeconds = 10;
        [Export] private readonly NodePath pointTimerNodePath = null;
        private Timer pointTimer;
        [Export] private readonly NodePath addEnergyTimerNodePath = null;
        private Timer addEnergyTimer;
        [Export] private readonly NodePath treeActionRadiusNodePath = null;
        private TreeActionRadius treeActionRadius;
        [Export] private readonly NodePath treeStateNodePath = null;
        private TreeState treeState;
        [Export] private readonly PackedScene addEnergyParticleEffectPackedScene = null;
        private GameState gameState;
        private Player player;

        public override void _Ready() {
            base._Ready();
            gameState = NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true);
            player = NodeGetter.GetFirstNodeInGroup<Player>(GetTree(), GameConstants.PlayerGroup, true);

            pointTimer = GetNode<Timer>(pointTimerNodePath);
            treeState = GetNode<TreeState>(treeStateNodePath);
            addEnergyTimer = GetNode<Timer>(addEnergyTimerNodePath);
            treeActionRadius = GetNode<TreeActionRadius>(treeActionRadiusNodePath);
            pointTimer.Connect("timeout", this, nameof(OnPointTimerTimeout));
            addEnergyTimer.Connect("timeout", this, nameof(OnEnergyTimerTimeout));
        }

        public override void Enter(IStateMachineMessage message = null) {
            pointTimer.Start(addPointsTimeIntervalInSeconds);
            addEnergyTimer.Start(GameValues.treeAddEnergyPerSecondsInterval);
        }

        public override void UnhandledInput(InputEvent @event) { }

        public override void PhysicsProcess(float delta) { }

        public override void Exit() {
            pointTimer.Stop();
            addEnergyTimer.Stop();
        }

        public override string GetName() {
            return "FullyGrownState";
        }

        private void OnPointTimerTimeout() {
            gameState.AddPlayerPoints(GameValues.pointsPerSecondForHealthyTree * treeState.treeGrowth);
        }

        private void OnEnergyTimerTimeout() {
            if (!treeActionRadius.IsEntityInActionRadius(player)) return;
            if (!(addEnergyParticleEffectPackedScene.Instance() is TreeAddEnergyParticleEffect addEnergyParticleEffect)) return;
            addEnergyParticleEffect.Init(player);
            addEnergyParticleEffect.SetGlobalPosition(GetGlobalPosition());
            AddChild(addEnergyParticleEffect);
        }
    }
}
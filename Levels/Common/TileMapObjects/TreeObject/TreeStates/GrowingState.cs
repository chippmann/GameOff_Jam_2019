using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common.StateMachine;
using GameOff_2019.Entities.PlayerEntity;
using GameOff_2019.Levels.Common.TileMapObjects.TreeObject.Effects;
using GameOff_2019.RoundLogic;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.TreeObject.TreeStates {
    public class GrowingState : State {
        [Export] private readonly int initialHealth = 10;
        [Export] private readonly int treeGrowthRatePerMinute = 5;
        [Export] private readonly int treeGrowthValue = 5;
        [Export] private readonly int treeHealthIncreaseValue = 5;
        [Export] private readonly int addPointsIntervalInSeconds = 5;
        [Export] private readonly int maxTreeGrowthValue = 100; //be careful with values over 100 as they apply 1:1 to the scale of the treeSprite!
        [Export] private readonly NodePath treeStateNodePath = null;
        private TreeState treeState;
        [Export] private readonly NodePath treeSpriteNodePath = null;
        private Sprite treeSprite;
        [Export] private readonly NodePath transparencyCollisionShapeNodePath = null;
        private CollisionShape2D transparencyCollisionShape;
        [Export] private readonly NodePath treeGrowthTimerNodePath = null;
        private Timer treeGrowthTimer;
        [Export] private readonly NodePath pointTimerNodePath = null;
        private Timer pointTimer;
        [Export] private readonly NodePath addEnergyTimerNodePath = null;
        private Timer addEnergyTimer;
        [Export] private readonly NodePath treeActionRadiusNodePath = null;
        private TreeActionRadius treeActionRadius;
        [Export] private readonly PackedScene addEnergyParticleEffectPackedScene = null;
        private GameState gameState;

        private readonly Vector2 finalScale = new Vector2(1, 1);
        private Player player;

        public override void _Ready() {
            base._Ready();
            gameState = NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true);
            player = NodeGetter.GetFirstNodeInGroup<Player>(GetTree(), GameConstants.PlayerGroup, true);

            treeState = GetNode<TreeState>(treeStateNodePath);
            treeSprite = GetNode<Sprite>(treeSpriteNodePath);
            transparencyCollisionShape = GetNode<CollisionShape2D>(transparencyCollisionShapeNodePath);
            treeGrowthTimer = GetNode<Timer>(treeGrowthTimerNodePath);
            pointTimer = GetNode<Timer>(pointTimerNodePath);
            addEnergyTimer = GetNode<Timer>(addEnergyTimerNodePath);
            treeActionRadius = GetNode<TreeActionRadius>(treeActionRadiusNodePath);
            pointTimer.Connect("timeout", this, nameof(OnPointTimerTimeout));
            treeGrowthTimer.Connect("timeout", this, nameof(OnTreeGrowthTimerTimeout));
            addEnergyTimer.Connect("timeout", this, nameof(OnEnergyTimerTimeout));
        }

        public override void Enter(IStateMachineMessage message = null) {
            if (treeState.treeHealth < initialHealth) {
                treeState.treeHealth = initialHealth;
            }

            treeGrowthTimer.Start(60f / treeGrowthRatePerMinute);
            pointTimer.Start(addPointsIntervalInSeconds);
            addEnergyTimer.Start(GameValues.treeAddEnergyPerSecondsInterval);
        }

        public override void UnhandledInput(InputEvent @event) { }

        public override void PhysicsProcess(float delta) {
            treeSprite.Scale = new Vector2((treeState.treeGrowth / 100f) * finalScale.x, (treeState.treeGrowth / 100f) * finalScale.y);
            transparencyCollisionShape.GetParent<Node2D>().Scale = new Vector2((treeState.treeGrowth / 100f) * finalScale.x, (treeState.treeGrowth / 100f) * finalScale.y);
        }

        public override void Exit() {
            treeGrowthTimer.Stop();
            pointTimer.Stop();
            addEnergyTimer.Stop();
        }

        public override string GetName() {
            return "TreeGrowingState";
        }

        private void OnTreeGrowthTimerTimeout() {
            if (treeState.treeGrowth + treeGrowthValue >= maxTreeGrowthValue) {
                treeState.treeGrowth = maxTreeGrowthValue;
                GetStateMachine<TreeStateMachine>().TransitionTo(GetStateMachine<TreeStateMachine>().fullGrown);
            }
            else {
                treeState.treeGrowth += treeGrowthValue;
            }

            treeState.treeHealth += treeHealthIncreaseValue;
        }

        private void OnPointTimerTimeout() {
            gameState.AddPlayerPoints(treeState.treeGrowth);
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
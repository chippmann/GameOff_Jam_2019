using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common.StateMachine;
using GameOff_2019.Entities.DemonEntity;
using GameOff_2019.Levels.Common.TileMapObjects.TreeObject.Effects;
using GameOff_2019.RoundLogic;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.TreeObject.TreeStates {
    public class InfestedState : State {
        [Export] private readonly int applyDamageIntervalInSeconds = 10;
        [Export] private readonly int addPointsTimeIntervalInSeconds = 10;
        [Export] private readonly NodePath pointTimerNodePath = null;
        private Timer pointTimer;
        [Export] private readonly NodePath damageTimerNodePath = null;
        private Timer damageTimer;
        [Export] private readonly NodePath addEnergyTimerNodePath = null;
        private Timer addEnergyTimer;
        [Export] private readonly NodePath treeActionRadiusNodePath = null;
        private TreeActionRadius treeActionRadius;
        [Export] private readonly NodePath treeStateNodePath = null;
        private TreeState treeState;
        [Export] private readonly PackedScene addEnergyParticleEffectPackedScene = null;
        private GameState gameState;
        private Demon demon;

        private int damageDealtSinceInfestion = 0;

        public override void _Ready() {
            base._Ready();
            gameState = NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true);
            demon = NodeGetter.GetFirstNodeInGroup<Demon>(GetTree(), GameConstants.DemonGroup, true);

            pointTimer = GetNode<Timer>(pointTimerNodePath);
            damageTimer = GetNode<Timer>(damageTimerNodePath);
            treeState = GetNode<TreeState>(treeStateNodePath);
            addEnergyTimer = GetNode<Timer>(addEnergyTimerNodePath);
            treeActionRadius = GetNode<TreeActionRadius>(treeActionRadiusNodePath);
            pointTimer.Connect("timeout", this, nameof(OnPointTimerTimeout));
            damageTimer.Connect("timeout", this, nameof(OnDamageTimerTimeout));
            addEnergyTimer.Connect("timeout", this, nameof(OnEnergyTimerTimeout));
        }

        public override void Enter(IStateMachineMessage message = null) {
            timePassed = 0;
            pointTimer.Start(addPointsTimeIntervalInSeconds);
            damageTimer.Start(applyDamageIntervalInSeconds);
            addEnergyTimer.Start(GameValues.treeAddEnergyPerSecondsInterval);
        }

        public override void UnhandledInput(InputEvent @event) { }

        private float timePassed;

        public override void PhysicsProcess(float delta) {
            timePassed += delta;
            if (timePassed >= 1) {
                timePassed = 0;
                gameState.AddDemonPoints(GameValues.pointsPerSecondForInfestedTree);
            }

            //TODO: show infested state on tree
        }

        public override void Exit() {
            pointTimer.Stop();
            damageTimer.Stop();
            addEnergyTimer.Stop();
            damageDealtSinceInfestion = 0;
        }

        public override string GetName() {
            return "InfestedState";
        }

        private void OnPointTimerTimeout() {
            gameState.AddDemonPoints(damageDealtSinceInfestion);
        }

        private void OnDamageTimerTimeout() {
            treeState.treeHealth -= GameValues.treeInfestionDamage;
            damageDealtSinceInfestion += GameValues.treeInfestionDamage;

            if (treeState.treeHealth <= 0) {
                gameState.AddDemonPoints(GameValues.killTreePoints);
                GetStateMachine<TreeStateMachine>().TransitionTo(GetStateMachine<TreeStateMachine>().dead);
            }
        }

        private void OnEnergyTimerTimeout() {
//            if (!treeActionRadius.IsEntityInActionRadius(demon)) return;
            if (!(addEnergyParticleEffectPackedScene.Instance() is TreeAddEnergyParticleEffect addEnergyParticleEffect)) return;
            addEnergyParticleEffect.Init(demon);
            addEnergyParticleEffect.SetGlobalPosition(GetGlobalPosition());
            AddChild(addEnergyParticleEffect);
        }
    }
}
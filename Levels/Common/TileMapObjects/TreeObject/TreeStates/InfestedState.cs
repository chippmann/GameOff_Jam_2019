using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common.StateMachine;
using GameOff_2019.RoundLogic;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.TreeObject.TreeStates {
    public class InfestedState : State {
        [Export] private readonly int damage = 5;
        [Export] private readonly int applyDamageIntervalInSeconds = 10;
        [Export] private readonly int addPointsTimeIntervalInSeconds = 10;
        [Export] private readonly NodePath pointTimerNodePath = null;
        private Timer pointTimer;
        [Export] private readonly NodePath damageTimerNodePath = null;
        private Timer damageTimer;
        [Export] private readonly NodePath treeStateNodePath = null;
        private TreeState treeState;
        private GameState gameState;

        private int damageDealtSinceInfestion = 0;

        public override void _Ready() {
            base._Ready();
            gameState = NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true);

            pointTimer = GetNode<Timer>(pointTimerNodePath);
            damageTimer = GetNode<Timer>(damageTimerNodePath);
            treeState = GetNode<TreeState>(treeStateNodePath);
            pointTimer.Connect("timeout", this, nameof(OnPointTimerTimeout));
            damageTimer.Connect("timeout", this, nameof(OnDamageTimerTimeout));
        }

        public override void Enter(IStateMachineMessage message = null) {
            pointTimer.Start(addPointsTimeIntervalInSeconds);
            damageTimer.Start(applyDamageIntervalInSeconds);
        }

        public override void UnhandledInput(InputEvent @event) { }

        public override void PhysicsProcess(float delta) {
            //TODO: show infested state on tree
        }

        public override void Exit() {
            pointTimer.Stop();
            damageTimer.Stop();
        }

        public override string GetName() {
            return "InfestedState";
        }

        private void OnPointTimerTimeout() {
            gameState.demonPoints += damageDealtSinceInfestion;
        }

        private void OnDamageTimerTimeout() {
            treeState.treeHealth -= damage;
            damageDealtSinceInfestion += damage;
            if (treeState.treeHealth <= 0) {
                GetStateMachine<TreeStateMachine>().TransitionTo(GetStateMachine<TreeStateMachine>().dead);
            }
        }
    }
}
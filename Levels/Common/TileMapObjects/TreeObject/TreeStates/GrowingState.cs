using System;
using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common.StateMachine;
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
        [Export] private readonly NodePath treeGrowthTimerNodePath = null;
        private Timer treeGrowthTimer;
        [Export] private readonly NodePath pointTimerNodePath = null;
        private Timer pointTimer;
        private GameState gameState;

        private Vector2 initialScale;

        public override void _Ready() {
            base._Ready();
            var gameStates = GetTree().GetNodesInGroup(GameConstants.GameStateGroup);
            if (gameStates.Count != 1) {
                throw new Exception("There should be exactly one gameState in the sceneTree!");
            }

            if (gameStates[0] is GameState) {
                gameState = gameStates[0] as GameState;
            }
            else {
                throw new Exception("Nodes in group \"" + GameConstants.GameStateGroup + "\" should always be of type \"GameState\"!");
            }

            treeState = GetNode<TreeState>(treeStateNodePath);
            treeSprite = GetNode<Sprite>(treeSpriteNodePath);
            treeGrowthTimer = GetNode<Timer>(treeGrowthTimerNodePath);
            pointTimer = GetNode<Timer>(pointTimerNodePath);
            pointTimer.Connect("timeout", this, nameof(OnPointTimerTimeout));
            treeGrowthTimer.Connect("timeout", this, nameof(OnTreeGrowthTimerTimeout));
            initialScale = treeSprite.Scale;
        }

        public override void Enter(IStateMachineMessage message = null) {
            if (treeState.treeHealth < initialHealth) {
                treeState.treeHealth = initialHealth;
            }

            treeGrowthTimer.Start(60f / treeGrowthRatePerMinute);
            pointTimer.Start(addPointsIntervalInSeconds);
        }

        public override void UnhandledInput(InputEvent @event) { }

        public override void PhysicsProcess(float delta) {
            treeSprite.Scale = new Vector2((treeState.treeGrowth / 100f) * initialScale.x, (treeState.treeGrowth / 100f) * initialScale.y);
        }

        public override void Exit() {
            treeGrowthTimer.Stop();
            pointTimer.Stop();
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
            gameState.playerPoints += treeState.treeGrowth;
        }
    }
}
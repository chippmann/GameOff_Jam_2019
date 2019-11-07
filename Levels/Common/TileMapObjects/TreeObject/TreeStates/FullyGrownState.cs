using System;
using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common.StateMachine;
using GameOff_2019.RoundLogic;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.TreeObject.TreeStates {
    public class FullyGrownState : State {
        [Export] private readonly int addPointsTimeIntervalInSeconds = 10;
        [Export] private readonly NodePath pointTimerNodePath = null;
        private Timer pointTimer;
        [Export] private readonly NodePath treeStateNodePath = null;
        private TreeState treeState;
        private GameState gameState;

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

            pointTimer = GetNode<Timer>(pointTimerNodePath);
            treeState = GetNode<TreeState>(treeStateNodePath);
            pointTimer.Connect("timeout", this, nameof(OnPointTimerTimeout));
        }

        public override void Enter(IStateMachineMessage message = null) {
            pointTimer.Start(addPointsTimeIntervalInSeconds);
        }

        public override void UnhandledInput(InputEvent @event) { }

        public override void PhysicsProcess(float delta) { }

        public override void Exit() {
            pointTimer.Stop();
        }

        public override string GetName() {
            return "FullyGrownState";
        }

        private void OnPointTimerTimeout() {
            gameState.playerPoints += treeState.treeGrowth;
        }
    }
}
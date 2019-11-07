using System;
using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common.Navigation;
using GameOff_2019.Entities.Common.StateMachine;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.TreeObject.TreeStates {
    public class DeadState : State {
        private PathfindingTileMap pathfindingTileMap;

        public override void _Ready() {
            base._Ready();
            var tileMaps = GetTree().GetNodesInGroup(GameConstants.PathfindingTileMapGroup);
            if (tileMaps.Count != 1) {
                throw new Exception("There should be exactly one pathfindingTileMap in the sceneTree!");
            }

            if (tileMaps[0] is PathfindingTileMap) {
                pathfindingTileMap = tileMaps[0] as PathfindingTileMap;
            }
            else {
                throw new Exception("Nodes in group \"pathfindingTileMap\" should always be of type \"PathfindingTileMap\"!");
            }
        }

        public override void Enter(IStateMachineMessage message = null) {
            //TODO: show death animation
            //TODO: after animation finish:
            pathfindingTileMap.tileMapManipulator.DeleteTree(GetGlobalPosition());
        }

        public override void UnhandledInput(InputEvent @event) { }

        public override void PhysicsProcess(float delta) { }

        public override void Exit() { }

        public override string GetName() {
            return "DeadState";
        }
    }
}
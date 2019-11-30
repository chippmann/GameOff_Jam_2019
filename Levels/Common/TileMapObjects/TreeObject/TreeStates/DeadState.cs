using Godot;
using Planty.EngineUtils;
using Planty.Entities.Common.Navigation;
using Planty.Entities.Common.StateMachine;
using Planty.Entities.PlayerEntity;

namespace Planty.Levels.Common.TileMapObjects.TreeObject.TreeStates {
    public class DeadState : State {
        private PathfindingTileMap pathfindingTileMap;

        public override void _Ready() {
            base._Ready();
            pathfindingTileMap = NodeGetter.GetFirstNodeInGroup<PathfindingTileMap>(GetTree(), GameConstants.PathfindingTileMapGroup, true);
        }

        public override void Enter(IStateMachineMessage message = null) {
            //TODO: show death animation
            //TODO: after animation finish:
            var isPlayerInActionRadius = GetOwner<TreeTileMapObject>().EntityInActionRadius(NodeGetter.GetFirstNodeInGroup<Player>(GetTree(), GameConstants.PlayerGroup, true));
            pathfindingTileMap.tileMapManipulator.DeleteTree(GetGlobalPosition());
            if (isPlayerInActionRadius) {
                pathfindingTileMap.tileMapManipulator.CheckLoosingCondition();
            }
        }

        public override void UnhandledInput(InputEvent @event) { }

        public override void PhysicsProcess(float delta) { }

        public override void Exit() { }

        public override string GetName() {
            return "DeadState";
        }
    }
}
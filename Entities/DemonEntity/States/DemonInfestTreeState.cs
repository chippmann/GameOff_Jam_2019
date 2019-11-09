using System;
using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common;
using GameOff_2019.Entities.Common.Movement;
using GameOff_2019.Entities.Common.Navigation;
using GameOff_2019.Entities.Common.StateMachine;
using GameOff_2019.Entities.PlayerEntity.States.Message;
using GameOff_2019.Levels.Common.TileMapObjects.TreeObject;
using Godot;

namespace GameOff_2019.Entities.DemonEntity.States {
    public class DemonInfestTreeState : State {
        [Export] private readonly NodePath entityMovementNodePath = null;
        private EntityMovement entityMovement;
        private Vector2 targetPosition;
        private PathfindingTileMap pathfindingTileMap;

        public override void _Ready() {
            base._Ready();
            entityMovement = GetNode<EntityMovement>(entityMovementNodePath);

            pathfindingTileMap = NodeGetter.GetFirstNodeInGroup<PathfindingTileMap>(GetTree(), GameConstants.PathfindingTileMapGroup, true);
        }

        public override void Enter(IStateMachineMessage message = null) {
            if (!(message is MoveToPositionMessage)) {
                throw new Exception("State message is not of Type \"MoveToPositionMessage\"");
            }

            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.DemonTargetReached), this, nameof(TargetReached));
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.TargetCannotBeReached), this, nameof(TargetCannotBeReached));
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.InvalidateDemonPath), this, nameof(OnPathInvalidated));
            targetPosition = ((MoveToPositionMessage) message).GetTargetPosition();
            OnPathInvalidated();
        }

        public override void UnhandledInput(InputEvent @event) { }

        public override void PhysicsProcess(float delta) { }

        public override void Exit() {
            entityMovement.StopMovement();
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.DemonTargetReached), this, nameof(TargetReached));
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.TargetCannotBeReached), this, nameof(TargetCannotBeReached));
            if (GetNode<Eventing>(Eventing.EventingNodePath).IsConnected(nameof(Eventing.InvalidateDemonPath), this, nameof(OnPathInvalidated))) {
                GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.InvalidateDemonPath), this, nameof(OnPathInvalidated));
            }
        }

        public override string GetName() {
            return "DemonInfestTreeState";
        }

        private void TargetReached() {
            if (GetNode<Eventing>(Eventing.EventingNodePath).IsConnected(nameof(Eventing.InvalidateDemonPath), this, nameof(OnPathInvalidated))) {
                GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.InvalidateDemonPath), this, nameof(OnPathInvalidated));
            }

            var tileMapObject = pathfindingTileMap.tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(pathfindingTileMap.WorldToMap(targetPosition));
            if (tileMapObject is TreeTileMapObject treeTileMapObject) {
                //TODO: show animation
                treeTileMapObject.Infest();
            }

            GetStateMachine<DemonStateMachine>().TransitionTo(GetStateMachine<DemonStateMachine>().idle);
        }

        private void TargetCannotBeReached(Entity sourceEntity) {
            if (sourceEntity is Demon) {
                GetStateMachine<DemonStateMachine>().TransitionTo(GetStateMachine<DemonStateMachine>().idle);
            }
        }

        private void OnPathInvalidated() {
            var tilePositionNextToTree = ((TreeTileMapObject) pathfindingTileMap.tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(targetPosition)).GetTilePositionNextToTree();
            if (tilePositionNextToTree != new Vector2(-1, -1)) {
                entityMovement.MoveToPosition(pathfindingTileMap.MapToWorld(tilePositionNextToTree), isPlayer: false, paramsToReturn: new object[] { }, targetCannotBeReachedParamsToReturn: new object[] {GetOwner<Demon>()});
            }
            else {
                GetStateMachine<DemonStateMachine>().TransitionTo(GetStateMachine<DemonStateMachine>().idle);
            }
        }
    }
}
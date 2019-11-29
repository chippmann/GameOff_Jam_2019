using System;
using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common;
using GameOff_2019.Entities.Common.Movement;
using GameOff_2019.Entities.Common.Navigation;
using GameOff_2019.Entities.Common.StateMachine;
using GameOff_2019.Entities.PlayerEntity.States.Message;
using GameOff_2019.Levels.Common.TileMapObjects.TreeObject;
using GameOff_2019.RoundLogic;
using GameOff_2019.SoundEngine;
using Godot;

namespace GameOff_2019.Entities.PlayerEntity.States {
    public class PlayerHealTreeState : State {
        [Export] private readonly NodePath entityMovementNodePath = null;
        [Export] private AudioStreamOGGVorbis playerWalkSound = null;
        [Export] private AudioStreamSample healTree = null;
        private EntityMovement entityMovement;
        private Vector2 targetPosition;
        private PathfindingTileMap pathfindingTileMap;
        private SoundEngineNode soundEngineNode;
        private AudioStreamPlayer soundPlayer;

        private GameState gameState;

        public override void _Ready() {
            base._Ready();
            entityMovement = GetNode<EntityMovement>(entityMovementNodePath);
            gameState = NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true);

            pathfindingTileMap = NodeGetter.GetFirstNodeInGroup<PathfindingTileMap>(GetTree(), GameConstants.PathfindingTileMapGroup, true);
            soundEngineNode = NodeGetter.GetFirstNodeInGroup<SoundEngineNode>(GetTree(), GameConstants.SoundEngineGroup, true);
        }

        public override void Enter(IStateMachineMessage message = null) {
            if (!(message is MoveToPositionMessage)) {
                throw new Exception("State message is not of Type \"MoveToPositionMessage\"");
            }
            
            soundPlayer = soundEngineNode.PlaySfxLoop(playerWalkSound);

            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.PlayerTargetReached), this, nameof(TargetReached));
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.TargetCannotBeReached), this, nameof(PlayerTargetCannotBeReached));
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.InvalidatePlayerPath), this, nameof(OnPathInvalidated));
            targetPosition = ((MoveToPositionMessage) message).GetTargetPosition();
            OnPathInvalidated();
        }

        public override void UnhandledInput(InputEvent @event) { }

        public override void PhysicsProcess(float delta) { }

        public override void Exit() {
            entityMovement.StopMovement();
            
            soundEngineNode.StopSfx(soundPlayer);
            
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.PlayerTargetReached), this, nameof(TargetReached));
            GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.TargetCannotBeReached), this, nameof(PlayerTargetCannotBeReached));
            if (GetNode<Eventing>(Eventing.EventingNodePath).IsConnected(nameof(Eventing.InvalidatePlayerPath), this, nameof(OnPathInvalidated))) {
                GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.InvalidatePlayerPath), this, nameof(OnPathInvalidated));
            }
        }

        public override string GetName() {
            return "PlayerHealTreeState";
        }

        private void TargetReached() {
            if (GetNode<Eventing>(Eventing.EventingNodePath).IsConnected(nameof(Eventing.InvalidatePlayerPath), this, nameof(OnPathInvalidated))) {
                GetNode<Eventing>(Eventing.EventingNodePath).Disconnect(nameof(Eventing.InvalidatePlayerPath), this, nameof(OnPathInvalidated));
            }
            
            soundEngineNode.PlaySfx(healTree);

            var tileMapObject = pathfindingTileMap.tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(targetPosition);
            if (tileMapObject is TreeTileMapObject treeTileMapObject) {
                //TODO: show animation
                treeTileMapObject.Heal();
                gameState.UsePlayerEnergy(GameValues.healTreeEnergyUsage);
                gameState.AddPlayerPoints(GameValues.healTreePoints);
            }

            GetStateMachine<PlayerStateMachine>().TransitionTo(GetStateMachine<PlayerStateMachine>().idle);
        }

        private void PlayerTargetCannotBeReached(Entity sourceEntity) {
            if (sourceEntity is Player) {
                GetStateMachine<PlayerStateMachine>().TransitionTo(GetStateMachine<PlayerStateMachine>().idle);
            }
        }

        private void OnPathInvalidated() {
            var tilePositionNextToTree = ((TreeTileMapObject) pathfindingTileMap.tileMapManipulator.GetTileMapObjectWithTileMapCoordinates(targetPosition)).GetTilePositionNextToTree();
            if (tilePositionNextToTree != new Vector2(-1, -1)) {
                entityMovement.MoveToPosition(pathfindingTileMap.MapToWorld(tilePositionNextToTree), isPlayer: true, paramsToReturn: new object[] { }, targetCannotBeReachedParamsToReturn: new object[] {GetOwner<Player>()});
            }
            else {
                GetStateMachine<PlayerStateMachine>().TransitionTo(GetStateMachine<PlayerStateMachine>().idle);
            }
        }
    }
}
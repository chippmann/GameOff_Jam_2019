using GameOff_2019.EngineUtils;
using GameOff_2019.Entities.Common;
using GameOff_2019.Entities.PlayerEntity;
using GameOff_2019.RoundLogic;
using Godot;

namespace GameOff_2019.Levels.Common.TileMapObjects.TreeObject.Effects {
    public class TreeAddEnergyParticleEffect : Particles2D {
        [Export] private readonly int speedInTileMaps = 4;
        [Export] private readonly Texture playerTexture = null;
        [Export] private readonly Texture demonTexture = null;
        private int internalMaxMovementSpeed;
        private Entity targetToFollow;
        private GameState gameState;
        private Tween positionTween;

        private Vector2 tmpTargetPosition;

        public override void _Ready() {
            base._Ready();
            gameState = NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true);
            internalMaxMovementSpeed = speedInTileMaps * GameConstants.GameUnitSize;
            Texture = targetToFollow is Player ? playerTexture : demonTexture;
            positionTween = new Tween();
            AddChild(positionTween);
            SetGlobalPosition(GetParent<Node2D>().GetGlobalPosition());
        }

        public void Init(Entity target) {
            targetToFollow = target;
        }

        public override async void _Process(float delta) {
            base._Process(delta);
            if (GetGlobalPosition().DistanceTo(targetToFollow.GetGlobalPosition()) < GameConstants.GameUnitSize / 2f) {
                if (targetToFollow is Player) {
                    gameState.AddPlayerEnergy(GameValues.energyForEntitiesInRadius);
                }
                else {
                    gameState.AddDemonEnergy(GameValues.energyForEntitiesInRadius);
                }

                Emitting = false;
                var timer = new Timer {OneShot = true};
                AddChild(timer);
                timer.Start(GetLifetime());
                await ToSignal(timer, "timeout");
                QueueFree();
            }
            else if (tmpTargetPosition != targetToFollow.GetGlobalPosition()) {
                tmpTargetPosition = targetToFollow.GetGlobalPosition();
                positionTween.InterpolateMethod(this, "set_global_position", GetGlobalPosition(), targetToFollow.GetGlobalPosition(), Lifetime / 2, Tween.TransitionType.Sine, Tween.EaseType.Out);
                positionTween.Start();
            }
        }
    }
}
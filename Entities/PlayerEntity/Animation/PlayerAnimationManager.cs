using System;
using GameOff_2019.Entities.Common.Movement;
using Godot;

namespace GameOff_2019.Entities.PlayerEntity.Animation {
    public class PlayerAnimationManager : AnimatedSprite {
        [Export] private readonly NodePath entityMovementNodePath = null;
        private EntityMovement entityMovement;

        public override void _Ready() {
            base._Ready();
            entityMovement = GetNode<EntityMovement>(entityMovementNodePath);
        }

        public override void _Process(float delta) {
            base._Process(delta);

            var velocity = entityMovement.GetVelocity();

            if (velocity == Vector2.Zero && IsPlaying()) {
                Stop();
            }
            else if (velocity != Vector2.Zero && !IsPlaying()) {
                var animationToPlay = GetAnimationToPlay(velocity);
                if (animationToPlay == GetAnimation()) {
                    Play();
                }
                else {
                    Play(animationToPlay);
                }
            }
        }

        private string GetAnimationToPlay(Vector2 velocity) {
            if (Math.Abs(velocity.x) < 0.01 && velocity.y < -0.2) { //up
                return "up";
            }

            if (Math.Abs(velocity.x) < 0.01 && velocity.y > 0.2) { //down
                return "down";
            }

            if (velocity.x < -0.2 && Math.Abs(velocity.y) < 0.01) { //left
                return "left";
            }

            if (velocity.x > 0.2 && Math.Abs(velocity.y) < 0.01) { //right
                return "right";
            }

            if (velocity.x < -0.2 && velocity.y < -0.2) { //upLeft
                return "upLeft";
            }

            if (velocity.x > 0.2 && velocity.y < -0.2) { //upRight
                return "upRight";
            }

            if (velocity.x < -0.2 && velocity.y > 0.2) { //downLeft
                return "downLeft";
            }

            if (velocity.x > 0.2 && velocity.y > 0.2) { //downRight
                return "downRight";
            }

            return GetAnimation();
        }
    }
}
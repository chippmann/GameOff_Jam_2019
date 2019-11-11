using System;
using System.Collections.Generic;
using GameOff_2019.EngineUtils;
using Godot;

namespace GameOff_2019.Entities.Common.Movement {
    public class EntityMovement : Node2D {
        [Export] private readonly NodePath entityBodyNodePath = null;
        private Entity entity;
        [Export] private readonly int movementSpeed = 3;
        [Export] private readonly int maxMovementSpeed = 3;
        private int internalMaxMovementSpeed;
        private object[] callbackParams;
        private object[] targetCannotBeReachedParams;

        private bool internalIsPlayer = false;
        private Vector2 velocity;
        private MovementState currentMovementState = MovementState.None;
        private readonly bool[] movementInputs = new bool[4];
        private List<Vector2> currentPathToFollow;
        private int currentTargetNode = -1;
        private int stuckFrames;
        private readonly int maxStuckFrames = 20;
        private readonly int entityMaxPositionErrorInPixels = 2;
        private Vector2 oldPosition;


        private enum MovementInput {
            Left = 0,
            Right = 1,
            Up = 2,
            Down = 3
        }

        private enum MovementState {
            None = 0,
            MoveTo
        }

        public override void _Ready() {
            entity = GetNode<Entity>(entityBodyNodePath);
            internalMaxMovementSpeed = maxMovementSpeed * GameConstants.GameUnitSize;
        }

        public override void _Process(float delta) {
            if (currentPathToFollow != null && currentPathToFollow.Count <= 1) {
                currentMovementState = MovementState.None;
                currentTargetNode = -1;
            }

            switch (currentMovementState) {
                case MovementState.None:
                    break;
                case MovementState.MoveTo:
                    Move();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UpdateEntityMovement(delta);
        }

        private void UpdateEntityMovement(float delta) {
            oldPosition = GetGlobalPosition();
            velocity = Vector2.Zero;

            for (var i = 0; i < movementInputs.Length; i++) {
                switch (i) {
                    case (int) MovementInput.Left: {
                        if (movementInputs[i]) {
                            velocity.x -= 1 * movementSpeed * GameConstants.GameUnitSize;
                        }

                        break;
                    }
                    case (int) MovementInput.Right: {
                        if (movementInputs[i]) {
                            velocity.x += 1 * movementSpeed * GameConstants.GameUnitSize;
                        }

                        break;
                    }
                    case (int) MovementInput.Up: {
                        if (movementInputs[i]) {
                            velocity.y -= 1 * movementSpeed * GameConstants.GameUnitSize;
                        }

                        break;
                    }
                    case (int) MovementInput.Down: {
                        if (movementInputs[i]) {
                            velocity.y += 1 * movementSpeed * GameConstants.GameUnitSize;
                        }

                        break;
                    }
                    default:
                        throw new ArgumentException("Unknown MovementInput " + movementInputs[i]);
                }
            }

            velocity.x = Mathf.Clamp(velocity.x, -internalMaxMovementSpeed, internalMaxMovementSpeed);
            velocity.y = Mathf.Clamp(velocity.y, -internalMaxMovementSpeed, internalMaxMovementSpeed);

            if (IsMovingDiagonally()) {
                velocity.x /= Mathf.Sqrt(Mathf.Pi);
                velocity.y /= Mathf.Sqrt(Mathf.Pi);
            }

            for (var i = 0; i < movementInputs.Length; i++) {
                movementInputs[i] = false;
            }

            velocity = entity.MoveAndSlide(velocity);
        }

        private bool IsMovingDiagonally() {
            return Math.Abs(velocity.x) > 0.1 && Math.Abs(velocity.y) > 0.1;
        }

        public void MoveToPosition(Vector2 targetPosition, object[] paramsToReturn = null, object[] targetCannotBeReachedParamsToReturn = null, bool isPlayer = false, bool minusOne = false) {
            internalIsPlayer = isPlayer;
            callbackParams = paramsToReturn;
            targetCannotBeReachedParams = targetCannotBeReachedParamsToReturn;
            MoveToTileMapPosition(targetPosition, minusOne);
        }

        private void MoveToTileMapPosition(Vector2 targetPosition, bool minusOne = false) {
            StopMovement();
            currentTargetNode = 1;
            stuckFrames = 0;
            var startTile = GetGlobalPosition();
            currentPathToFollow = internalIsPlayer ? entity.GetPathfinderTileMap().FindPathToTargetForPlayer(startTile, targetPosition) : entity.GetPathfinderTileMap().FindPathToTargetForDemon(startTile, targetPosition);
            if (minusOne && currentPathToFollow.Count > 0) {
                currentPathToFollow.RemoveAt(currentPathToFollow.Count - 1);
            }

            if (currentPathToFollow.Count == 0) {
                GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.TargetCannotBeReached), targetCannotBeReachedParams);
            }

            ResumeMovement();
        }

        public void StopMovement() {
            currentMovementState = MovementState.None;
        }

        public void ResumeMovement() {
            currentMovementState = MovementState.MoveTo;
        }


        private void Move() {
            GetEntityOnPathPositionSituation(out var previousDestination, out var currentDestination, out var nextDestination, out var reachedCurrentDestinationsX, out var reachedCurrentDestinationsY);
            CheckIfStuck();
            if (reachedCurrentDestinationsX && reachedCurrentDestinationsY) {
                currentTargetNode++;
                if (currentTargetNode >= currentPathToFollow.Count) {
                    currentTargetNode = -1;
                    StopMovement();
                    if (internalIsPlayer) {
                        GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.PlayerTargetReached), callbackParams);
                    }
                    else {
                        GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.DemonTargetReached), callbackParams);
                    }

                    return;
                }
            }
            else {
                if (currentDestination.x - GetGlobalPosition().x > entityMaxPositionErrorInPixels) {
                    movementInputs[(int) MovementInput.Right] = true;
                }
                else if (GetGlobalPosition().x - currentDestination.x > entityMaxPositionErrorInPixels) {
                    movementInputs[(int) MovementInput.Left] = true;
                }

                if (currentDestination.y - GetGlobalPosition().y > entityMaxPositionErrorInPixels) {
                    movementInputs[(int) MovementInput.Down] = true;
                }
                else if (GetGlobalPosition().y - currentDestination.y > entityMaxPositionErrorInPixels) {
                    movementInputs[(int) MovementInput.Up] = true;
                }
            }
        }

        private void CheckIfStuck() {
            if (GetGlobalPosition() == oldPosition) {
                stuckFrames++;
                if (stuckFrames > maxStuckFrames) {
                    MoveToPosition(currentPathToFollow[currentPathToFollow.Count - 1], callbackParams, targetCannotBeReachedParams, internalIsPlayer);
                }
            }
            else {
                stuckFrames = 0;
            }
        }

        private void GetEntityOnPathPositionSituation(out Vector2 previousDestination, out Vector2 currentDestination, out Vector2 nextDestination, out bool reachedCurrentDestinationX, out bool reachedCurrentDestinationY) {
            previousDestination = new Vector2(currentPathToFollow[currentTargetNode - 1]);
            currentDestination = new Vector2(currentPathToFollow[currentTargetNode]);
            nextDestination = currentDestination;

            if (currentPathToFollow.Count > currentTargetNode + 1) {
                nextDestination = new Vector2(currentPathToFollow[currentTargetNode + 1]);
            }

            reachedCurrentDestinationX = ReachedNodeOnXAxis(previousDestination, currentDestination);
            reachedCurrentDestinationY = ReachedNodeOnYAxis(previousDestination, currentDestination);
        }

        private bool ReachedNodeOnXAxis(Vector2 previousDestination, Vector2 currentDestination) {
            return (previousDestination.x <= currentDestination.x && Mathf.Round(GetGlobalPosition().x + entityMaxPositionErrorInPixels) >= currentDestination.x)
                   || (previousDestination.x >= currentDestination.x && Mathf.Round(GetGlobalPosition().x - entityMaxPositionErrorInPixels) <= currentDestination.x);
        }

        private bool ReachedNodeOnYAxis(Vector2 previousDestination, Vector2 currentDestination) {
            return (previousDestination.y <= currentDestination.y && Mathf.Round(GetGlobalPosition().y + entityMaxPositionErrorInPixels) >= currentDestination.y)
                   || (previousDestination.y >= currentDestination.y && Mathf.Round(GetGlobalPosition().y - entityMaxPositionErrorInPixels) <= currentDestination.y);
        }
    }
}
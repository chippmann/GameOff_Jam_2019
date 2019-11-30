using System;
using Godot;
using Planty.EngineUtils;
using Planty.Entities.PlayerEntity;

namespace Planty.Ui {
    public class LevelMoveMouseControl : Control {
        [Export] private readonly NodePath cameraNodePath = null;
        private Camera2D camera;
        [Export] private readonly NodePath mouseCaptureLeftNodePath = null;
        private Control mouseCaptureLeft;
        [Export] private readonly NodePath mouseCaptureRightNodePath = null;
        private Control mouseCaptureRight;
        [Export] private readonly NodePath mouseCaptureTopNodePath = null;
        private Control mouseCaptureTop;
        [Export] private readonly NodePath mouseCaptureBottomNodePath = null;
        private Control mouseCaptureBottom;

        [Export] private readonly NodePath uiContainerCanvasLayerNodePath = null;

//        private CanvasLayer uiContainerCanvasLayer;
        [Export] private readonly int mouseSensitivity = 30;


        private readonly bool[] moveInput = new bool[4];

        private enum MoveInput {
            Left = 0,
            Right = 1,
            Up = 2,
            Down = 3
        }

        private Vector2 uiContainerInitialOffset;

        public override void _Ready() {
            camera = GetNode<Camera2D>(cameraNodePath);
            mouseCaptureLeft = GetNode<Control>(mouseCaptureLeftNodePath);
            mouseCaptureRight = GetNode<Control>(mouseCaptureRightNodePath);
            mouseCaptureTop = GetNode<Control>(mouseCaptureTopNodePath);
            mouseCaptureBottom = GetNode<Control>(mouseCaptureBottomNodePath);
//            uiContainerCanvasLayer = GetNode<CanvasLayer>(uiContainerCanvasLayerNodePath);
//            uiContainerInitialOffset = uiContainerCanvasLayer.GetOffset();
            SetInitialPosition();
        }

        private void SetInitialPosition() {
            var windowWidth = (int) ProjectSettings.GetSetting("display/window/size/width");
            var windowHeight = (int) ProjectSettings.GetSetting("display/window/size/height");
            var playerGlobalPosition = NodeGetter.GetFirstNodeInGroup<Player>(GetTree(), GameConstants.PlayerGroup, true).GetGlobalPosition();
            camera.SetGlobalPosition(playerGlobalPosition - new Vector2(windowWidth, windowHeight));
        }

        public override void _PhysicsProcess(float delta) {
            var positionChange = AreMoveInputsPressed() ? MoveWindowWithKeyboard() : MoveWindowWithMouse();
            var newCameraPosition = camera.GetPosition() + positionChange * mouseSensitivity;
            if (newCameraPosition.x < 0) {
                newCameraPosition.x = 0;
            }

            if (newCameraPosition.y < 0) {
                newCameraPosition.y = 0;
            }

            if (newCameraPosition.x > 10150) { //TODO: if we have some time left: replace this hardcoded values with calculated ones. Somehow `GameConstants.BackgroundImageWidth + 250 - GetViewport().GetVisibleRect().Size.x` isn't correct
                newCameraPosition.x = 10150;
            }

            if (newCameraPosition.y > 11280) { //TODO: if we have some time left: replace this hardcoded values with calculated ones. Somehow `GameConstants.BackgroundImageHeight - GetViewport().GetVisibleRect().Size.y` isn't correct
                newCameraPosition.y = 11280;
            }

            camera.SetPosition(newCameraPosition);
//            var transform = new Transform2D(uiContainerCanvasLayer.Transform.x, uiContainerCanvasLayer.Transform.y, -camera.GetGlobalPosition() + uiContainerInitialOffset);
//            uiContainerCanvasLayer.SetTransform(transform);
        }

        private Vector2 MoveWindowWithKeyboard() {
            return new Vector2(
                Input.GetActionStrength(GameConstants.ControlsMoveWindowRight) - Input.GetActionStrength(GameConstants.ControlsMoveWindowLeft),
                Input.GetActionStrength(GameConstants.ControlsMoveWindowDown) - Input.GetActionStrength(GameConstants.ControlsMoveWindowUp)
            );
        }

        private bool AreMoveInputsPressed() {
            return Input.IsActionPressed(GameConstants.ControlsMoveWindowLeft)
                   || Input.IsActionPressed(GameConstants.ControlsMoveWindowUp)
                   || Input.IsActionPressed(GameConstants.ControlsMoveWindowRight)
                   || Input.IsActionPressed(GameConstants.ControlsMoveWindowDown);
        }

        private Vector2 MoveWindowWithMouse() {
            var mousePosition = GetLocalMousePosition();
            var left = mouseCaptureLeft.GetSize().x;
            var top = mouseCaptureTop.GetSize().y;
            var bottom = mouseCaptureBottom.GetPosition().y - mouseCaptureBottom.GetSize().y;
            var right = mouseCaptureRight.GetPosition().x - mouseCaptureRight.GetSize().x;

            for (var i = 0; i < moveInput.Length; i++) {
                moveInput[i] = false;
            }

            if (mousePosition.x <= left) {
                moveInput[(int) MoveInput.Left] = true;
            }

            if (mousePosition.x >= right) {
                moveInput[(int) MoveInput.Right] = true;
            }

            if (mousePosition.y <= top) {
                moveInput[(int) MoveInput.Up] = true;
            }

            if (mousePosition.y >= bottom) {
                moveInput[(int) MoveInput.Down] = true;
            }

            var velocity = Vector2.Zero;

            for (var i = 0; i < moveInput.Length; i++) {
                switch (i) {
                    case (int) MoveInput.Left: {
                        if (moveInput[i]) {
                            velocity.x -= 1;
                        }

                        break;
                    }
                    case (int) MoveInput.Right: {
                        if (moveInput[i]) {
                            velocity.x += 1;
                        }

                        break;
                    }
                    case (int) MoveInput.Up: {
                        if (moveInput[i]) {
                            velocity.y -= 1;
                        }

                        break;
                    }
                    case (int) MoveInput.Down: {
                        if (moveInput[i]) {
                            velocity.y += 1;
                        }

                        break;
                    }
                    default:
                        throw new ArgumentException("Unknown MoveInput " + moveInput[i]);
                }
            }

            return velocity;
        }
    }
}
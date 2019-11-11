using System;
using GameOff_2019.EngineUtils;
using Godot;

namespace GameOff_2019.Ui {
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
        private CanvasLayer uiContainerCanvasLayer;
        [Export] private readonly int mouseSensitivity = 15;


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
            uiContainerCanvasLayer = GetNode<CanvasLayer>(uiContainerCanvasLayerNodePath);
            uiContainerInitialOffset = uiContainerCanvasLayer.GetOffset();
        }

        public override void _PhysicsProcess(float delta) {
            var positionChange = AreMoveInputsPressed() ? MoveWindowWithKeyboard() : MoveWindowWithMouse();
            camera.SetPosition(camera.GetPosition() + positionChange * mouseSensitivity);
            var transform = new Transform2D(uiContainerCanvasLayer.Transform.x, uiContainerCanvasLayer.Transform.y, -camera.GetGlobalPosition() + uiContainerInitialOffset);
            uiContainerCanvasLayer.SetTransform(transform);
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
            return Vector2.Zero; //TODO: DEBUG!!!! Remove!
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
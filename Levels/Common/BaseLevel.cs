using Godot;
using Planty.EngineUtils;
using Planty.Levels.Common.TileMapObjects;
using Planty.Ui.Menu.PauseMenu;

namespace Planty.Levels.Common {
    public class BaseLevel : Node2D {
        [Export] private readonly NodePath gameUiNodePath = null;
        private Control gameUi;
        [Export] private readonly NodePath tileMapManipulatorNodePath = null;
        private TileMapManipulator tileMapManipulator;
        [Export] private readonly NodePath pauseMenuContainerNodePath = null;
        private Control pauseMenuContainer;
        [Export] private readonly PackedScene pauseMenuPackedScene = null;

        public override void _Ready() {
            base._Ready();
            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.LevelSetupFinished));
            gameUi = GetNode<Control>(gameUiNodePath);
            pauseMenuContainer = GetNode<Control>(pauseMenuContainerNodePath);
        }

        public override void _Process(float delta) {
            base._Process(delta);
            if (!Input.IsActionJustPressed(GameConstants.ControlsPauseMenu)) return;

            if (pauseMenuContainer.GetChild(0) is PauseMenuManager) {
                pauseMenuContainer.GetChild(0).QueueFree();
            }
            else {
                pauseMenuContainer.AddChild(pauseMenuPackedScene.Instance());
            }
        }

        public void Setup() {
            tileMapManipulator = GetNode<TileMapManipulator>(tileMapManipulatorNodePath);
            tileMapManipulator.SetupTileMap();
        }

        public void SetLevelVisibility(bool isVisible) {
            SetVisible(isVisible);
            gameUi.SetVisible(isVisible);
        }
    }
}
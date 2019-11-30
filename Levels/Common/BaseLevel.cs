using GameOff_2019.EngineUtils;
using GameOff_2019.Levels.Common.TileMapObjects;
using Godot;

namespace GameOff_2019.Levels.Common {
    public class BaseLevel : Node2D {
        [Export] private readonly NodePath gameUiNodePath = null;
        private Control gameUi;
        [Export] private readonly NodePath tileMapMaipulatorNodePath = null;
        private TileMapManipulator tileMapMaipulator;

        public override void _Ready() {
            base._Ready();
            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.LevelSetupFinished));
            gameUi = GetNode<Control>(gameUiNodePath);
        }

        public void Setup() {
            tileMapMaipulator = GetNode<TileMapManipulator>(tileMapMaipulatorNodePath);
            tileMapMaipulator.SetupTileMap();
        }

        public void SetLevelVisibility(bool isVisible) {
            SetVisible(isVisible);
            gameUi.SetVisible(isVisible);
        }
    }
}
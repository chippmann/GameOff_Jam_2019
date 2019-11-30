using Godot;
using Planty.EngineUtils;
using Planty.Levels.Common.TileMapObjects;
using Planty.RoundLogic;
using Planty.SoundEngine;
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
        
        private SoundEngineNode soundEngineNode;


        public override void _Ready() {
            base._Ready();
            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.LevelSetupFinished));
            gameUi = GetNode<Control>(gameUiNodePath);
            pauseMenuContainer = GetNode<Control>(pauseMenuContainerNodePath);
            
            soundEngineNode = NodeGetter.GetFirstNodeInGroup<SoundEngineNode>(GetTree(), GameConstants.SoundEngineGroup, true);
            soundEngineNode.PlayMusic("rainforest-01");
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

        public void Setup(GameState gameState) {
            tileMapManipulator = GetNode<TileMapManipulator>(tileMapManipulatorNodePath);
            tileMapManipulator.SetupTileMap(gameState);
        }

        public void SetLevelVisibility(bool isVisible) {
            SetVisible(isVisible);
            gameUi.SetVisible(isVisible);
        }
    }
}
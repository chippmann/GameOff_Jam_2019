using GameOff_2019.Levels.Common;
using Godot;

namespace GameOff_2019.RoundLogic {
    public class GameManager : Control {
        [Export] private readonly NodePath levelContainerNodePath = null;
        private Node2D levelContainer;
        [Export] private readonly NodePath menuContainerNodePath = null;
        private Control menuContainer;
        [Export] private readonly PackedScene introPackedScene = null;
        [Export] private readonly PackedScene mainMenuPackedScene = null;
        [Export] private readonly PackedScene victoryMenuPackedScene = null;
        [Export] private readonly PackedScene looseMenuPackedScene = null;
        [Export] private readonly PackedScene levelPackedScene = null;

        private Node2D tmpLevelHolder = new Node2D();

        public override void _Ready() {
            base._Ready();
            levelContainer = GetNode<Node2D>(levelContainerNodePath);
            menuContainer = GetNode<Control>(menuContainerNodePath);

            menuContainer.AddChild(introPackedScene.Instance());
            var baseLevel = levelPackedScene.Instance() as BaseLevel;
            tmpLevelHolder.AddChild(baseLevel);
            baseLevel?.Setup();
        }
    }
}
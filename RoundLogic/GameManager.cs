using Godot;

namespace GameOff_2019.RoundLogic {
    public class GameManager: Control {
        [Export] private readonly NodePath levelContainerNodePath = null;
        private Node2D levelContainer;
        [Export] private readonly NodePath menuContainerNodePath = null;
        private Control menuContainer;
        [Export] private readonly PackedScene introPackedScene = null;
        [Export] private readonly PackedScene mainMenuPackedScene = null;
        [Export] private readonly PackedScene victoryMenuPackedScene = null;
        [Export] private readonly PackedScene looseMenuPackedScene = null;
        [Export] private readonly PackedScene levelPackedScene = null;
        
        public override void _Ready() {
            base._Ready();
            levelContainer = GetNode<Node2D>(levelContainerNodePath);
            menuContainer = GetNode<Control>(menuContainerNodePath);

            menuContainer.AddChild(introPackedScene.Instance());
            //menuContainer.CallDeferred("add_child", levelPackedScene.Instance());
        }
    }
}
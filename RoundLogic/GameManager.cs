using GameOff_2019.EngineUtils;
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
        [Export] private readonly PackedScene baseEndMenuPackedScene = null;
        [Export] private readonly PackedScene levelPackedScene = null;

        private Node2D tmpLevelHolder = new Node2D();
        private BaseLevel mainLevel;

        public override void _Ready() {
            base._Ready();
            levelContainer = GetNode<Node2D>(levelContainerNodePath);
            menuContainer = GetNode<Control>(menuContainerNodePath);
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.IntroFinished), this, nameof(OnIntroFinished));
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.StartGamePressed), this, nameof(OnStartGamePressed));
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.GameWon), this, nameof(OnGameWonOrGameOver));
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.GameOver), this, nameof(OnGameWonOrGameOver));
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.ContinuePressed), this, nameof(OnContinuePressed));

            menuContainer.AddChild(introPackedScene.Instance());
            SetupMainLevel();
        }

        private void SetupMainLevel() {
            mainLevel = levelPackedScene.Instance() as BaseLevel;
            tmpLevelHolder.AddChild(mainLevel);
            mainLevel?.Setup();
        }

        private void OnIntroFinished() {
            menuContainer.AddChild(mainMenuPackedScene.Instance());
        }

        private void OnStartGamePressed() {
            tmpLevelHolder.RemoveChild(mainLevel);
            levelContainer.AddChild(mainLevel);
        }

        private void OnGameWonOrGameOver() {
            mainLevel.QueueFree();
            menuContainer.AddChild(baseEndMenuPackedScene.Instance());
            SetupMainLevel(); //for the next round we can already prepare the level
        }

        private void OnContinuePressed() {
            menuContainer.AddChild(mainMenuPackedScene.Instance());
            NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true).QueueFree(); //reset game state
            AddChild(new GameState());
        }
    }
}
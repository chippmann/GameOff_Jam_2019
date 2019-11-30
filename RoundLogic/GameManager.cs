using Godot;
using Planty.EngineUtils;
using Planty.Levels.Common;

namespace Planty.RoundLogic {
    public class GameManager : Control {
        [Export] private readonly NodePath levelContainerNodePath = null;
        private Node2D levelContainer;
        [Export] private readonly NodePath menuContainerNodePath = null;
        private Control menuContainer;
        [Export] private readonly PackedScene introPackedScene = null;
        [Export] private readonly PackedScene mainMenuPackedScene = null;
        [Export] private readonly PackedScene wonMenuPackedScene = null;
        [Export] private readonly PackedScene lostEndMenuPackedScene = null;
        [Export] private readonly PackedScene levelPackedScene = null;

        private Node2D tmpLevelHolder = new Node2D();
        private BaseLevel mainLevel;
        private GameState gameState;

        public override void _Ready() {
            base._Ready();
            levelContainer = GetNode<Node2D>(levelContainerNodePath);
            menuContainer = GetNode<Control>(menuContainerNodePath);
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.IntroFinished), this, nameof(OnIntroFinished));
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.StartGamePressed), this, nameof(OnStartGamePressed));
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.GameWon), this, nameof(OnGameWon));
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.GameOver), this, nameof(OnGameOver));
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.ContinuePressed), this, nameof(OnContinuePressed));
            GetNode<Eventing>(Eventing.EventingNodePath).Connect(nameof(Eventing.FinishCurrentGame), this, nameof(OnFinishCurrentGamePressed));
            gameState = NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true);

            menuContainer.AddChild(introPackedScene.Instance());
            SetupMainLevel(gameState);
        }

        private void SetupMainLevel(GameState gameState) {
            mainLevel = levelPackedScene.Instance() as BaseLevel;
            tmpLevelHolder.AddChild(mainLevel);
            mainLevel?.Setup(gameState);
        }

        private void OnIntroFinished() {
            menuContainer.AddChild(mainMenuPackedScene.Instance());
        }

        private void OnStartGamePressed() {
            tmpLevelHolder.RemoveChild(mainLevel);
            levelContainer.AddChild(mainLevel);
        }

        private void OnGameWon() {
            mainLevel.QueueFree();
            menuContainer.AddChild(wonMenuPackedScene.Instance());
            SetupMainLevel(gameState); //for the next round we can already prepare the level
        }

        private void OnGameOver() {
            mainLevel.QueueFree();
            menuContainer.AddChild(lostEndMenuPackedScene.Instance());
            SetupMainLevel(gameState); //for the next round we can already prepare the level
        }

        private void OnContinuePressed() {
            menuContainer.AddChild(mainMenuPackedScene.Instance());
            gameState.QueueFree(); //reset game state
            gameState = new GameState();
            AddChild(gameState);
        }

        private void OnFinishCurrentGamePressed() {
            mainLevel.QueueFree();
            gameState.QueueFree(); //reset game state
            gameState = new GameState();
            AddChild(gameState);
            menuContainer.AddChild(mainMenuPackedScene.Instance());
            SetupMainLevel(gameState); //for the next round we can already prepare the level
        }
    }
}
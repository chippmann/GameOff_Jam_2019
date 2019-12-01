using Godot;
using Planty.EngineUtils;
using Planty.RoundLogic;
using Planty.SoundEngine;

namespace Planty.Ui.Menu.MainMenu {
    public class MainMenuManager : Control {
        [Export] private readonly NodePath startGameWithPointLimitButtonNodePath = null;
        private Button startWithPointLimitGameButton;
        [Export] private readonly NodePath startGameButtonNodePath = null;
        private Button startGameButton;
        [Export] private readonly NodePath quitGameButtonNodePath = null;
        private Button quitGameButton;

        private SoundEngineNode soundEngineNode;

        public override void _Ready() {
            base._Ready();
            startWithPointLimitGameButton = GetNode<Button>(startGameWithPointLimitButtonNodePath);
            startWithPointLimitGameButton.Connect("pressed", this, nameof(OnStartWithPointLimitPressed));
            startGameButton = GetNode<Button>(startGameButtonNodePath);
            startGameButton.Connect("pressed", this, nameof(StartGame));
            quitGameButton = GetNode<Button>(quitGameButtonNodePath);
            quitGameButton.Connect("pressed", this, nameof(OnQuitPressed));

            soundEngineNode = NodeGetter.GetFirstNodeInGroup<SoundEngineNode>(GetTree(), GameConstants.SoundEngineGroup, true);
            soundEngineNode.PlayMusic("intro_03");
        }

        private void OnStartWithPointLimitPressed() {
            NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true).isPointLimitEnabled = true;
            StartGame();
        }

        private void StartGame() {
            QueueFree();
            soundEngineNode.StopMusic();
            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.StartGamePressed));
        }

        private void OnQuitPressed() {
            GetTree().Quit();
        }
    }
}
using GameOff_2019.EngineUtils;
using GameOff_2019.SoundEngine;
using Godot;

namespace GameOff_2019.Ui.Menu.MainMenu {
    public class MainMenuManager : Control {
        [Export] private readonly NodePath startGameButtonNodePath = null;
        private Button startGameButton;
        [Export] private readonly NodePath quitGameButtonNodePath = null;
        private Button quitGameButton;

        private SoundEngineNode soundEngine;

        public override void _Ready() {
            base._Ready();
            startGameButton = GetNode<Button>(startGameButtonNodePath);
            startGameButton.Connect("pressed", this, nameof(OnStartPressed));
            quitGameButton = GetNode<Button>(quitGameButtonNodePath);
            quitGameButton.Connect("pressed", this, nameof(OnQuitPressed));

            soundEngine = NodeGetter.GetFirstNodeInGroup<SoundEngineNode>(GetTree(), GameConstants.SoundEngineGroup, true);
            soundEngine.PlayMusic("intro_03");
        }

        private void OnStartPressed() {
            QueueFree();
            soundEngine.StopMusic();
            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.StartGamePressed));
        }

        private void OnQuitPressed() {
            GetTree().Quit();
        }
    }
}
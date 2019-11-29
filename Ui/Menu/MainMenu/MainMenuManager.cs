using GameOff_2019.EngineUtils;
using Godot;
using GameOff_2019.SoundEngine;

namespace GameOff_2019.Ui.Menu.MainMenu {
    public class MainMenuManager: Control {
        [Export] private readonly NodePath startGameButtonNodePath = null;
        private Button startGameButton;
        [Export] private readonly NodePath quitGameButtonNodePath = null;
        private Button quitGameButton;
        private SoundEngineNode soundEngineNode;
        
        public override void _Ready() {
            base._Ready();
            startGameButton = GetNode<Button>(startGameButtonNodePath);
            startGameButton.Connect("pressed", this, nameof(OnStartPressed));
            quitGameButton = GetNode<Button>(quitGameButtonNodePath);
            quitGameButton.Connect("pressed", this, nameof(OnQuitPressed));            
            
            // soundEngineNode = NodeGetter.GetFirstNodeInGroup<SoundEngineNode>(GetTree(), GameConstants.SoundEngineGroup, true);
            // soundEngineNode.PlayMusic("intro_03", true);

        }
        
        public override void _ExitTree() {
            base._ExitTree();
            soundEngineNode.StopMusic();
        }

        private void OnStartPressed() {
            QueueFree();
        }

        private void OnQuitPressed() {
            GetTree().Quit();
        }
    }
}
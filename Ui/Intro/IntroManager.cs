using GameOff_2019.EngineUtils;
using Godot;

namespace GameOff_2019.Ui.Intro {
    public class IntroManager : Control {
        [Export] private readonly NodePath videoPlayerNodePath = null;
        private VideoPlayer videoPlayer;
        [Export] private readonly NodePath skipButtonNodePath = null;
        private Button skipButton;
        [Export] private readonly PackedScene mainMenuSceneScene = null;

        public override void _Ready() {
            base._Ready();
            videoPlayer = GetNode<VideoPlayer>(videoPlayerNodePath);
            videoPlayer.Connect("finished", this, nameof(OnVideoFinished));
            skipButton = GetNode<Button>(skipButtonNodePath);
            skipButton.Connect("pressed", this, nameof(OnSkipPressed));

            videoPlayer.Play();
        }

        public override void _ExitTree() {
            base._ExitTree();
            videoPlayer.Disconnect("finished", this, nameof(OnVideoFinished));
        }

        private async void OnSkipPressed() {
            skipButton.SetDisabled(true);
            var fadeOutTween = new Tween();
            var fadeDownTween = new Tween();
            AddChild(fadeOutTween);
            AddChild(fadeDownTween);
            fadeOutTween.InterpolateMethod(videoPlayer, "set_modulate", videoPlayer.Modulate,
                new Color(videoPlayer.Modulate.r, videoPlayer.Modulate.g, videoPlayer.Modulate.b, 0), 2f, Tween.TransitionType.Sine, Tween.EaseType.InOut);
            fadeDownTween.InterpolateMethod(videoPlayer, "set_volume", videoPlayer.GetVolume(), 0, 2f,
                Tween.TransitionType.Sine, Tween.EaseType.InOut);
            fadeOutTween.Start();
            fadeDownTween.Start();

            await ToSignal(fadeOutTween, "tween_all_completed");
            await ToSignal(fadeDownTween, "tween_all_completed");

            OnVideoFinished();
        }

        private void OnVideoFinished() {
            QueueFree();
            GetNode<Eventing>(Eventing.EventingNodePath).EmitSignal(nameof(Eventing.IntroFinished));
        }
    }
}
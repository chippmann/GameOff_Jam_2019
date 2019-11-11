using Godot;

namespace GameOff_2019.Ui.TwitterUi {
    public class TwitterFeed : VBoxContainer {
        [Export] private readonly PackedScene tweetPackedScene;
        [Export] private readonly PackedScene tweetWithImagePackedScene;
    }
}
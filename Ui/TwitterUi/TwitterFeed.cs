using System.Collections.Generic;
using GameOff_2019.Data;
using GameOff_2019.Serialization;
using GameOff_2019.Ui.TwitterUi.Dynamic;
using Godot;

namespace GameOff_2019.Ui.TwitterUi {
    public class TwitterFeed : VBoxContainer {
        [Export] private readonly PackedScene tweetPackedScene;
        [Export] private readonly PackedScene tweetWithImagePackedScene;
        [Export] private readonly PackedScene fillerTweetPackedScene;
        [Export] private readonly string pathToTweetsJson = "res://Ui/TwitterUi/DebugTwitterJsonResponse.json";
        [Export] private readonly NodePath tmpTweetContainerNodePath = null;
        private Control tmpTweetContainer;

        private List<Tweet> tweets;

        private List<long> shownTweets = new List<long>();

        public override void _Ready() {
            base._Ready();
            tmpTweetContainer = GetNode<Control>(tmpTweetContainerNodePath);
            ReadTweetsFromDebugJson();
        }

        private int count = 0;
        private float timeElapsed;

        public override void _Process(float delta) {
            base._Process(delta);
            timeElapsed += delta;
            if (timeElapsed >= 2 /* && count < tweets.Count*/) {
                timeElapsed = 0;
                AddTweetToFeed(tweets[count]);
//                count++;
            }
        }

        private void ReadTweetsFromDebugJson() {
            var file = new File();
            file.Open(pathToTweetsJson, (int) File.ModeFlags.Read);
            var json = file.GetAsText();
            tweets = Serializer.Deserialize<List<Tweet>>(json);
        }

        private async void AddTweetToFeed(Tweet tweet) {
            if (!(tweetPackedScene.Instance() is TweetUi tweetUi)) return;
            if (!(fillerTweetPackedScene.Instance() is FillerTweet fillerTweet)) return;
            var animationTween = new Tween();
            AddChild(animationTween);

            shownTweets.Add(tweet.id);

            tmpTweetContainer.AddChild(tweetUi);
            tweetUi.Init(tweet);

            AddChild(fillerTweet);
            MoveChild(fillerTweet, 0);

            animationTween.InterpolateMethod(fillerTweet, "set_custom_minimum_size", Vector2.Zero, new Vector2(0, tweetUi.GetSize().y), 0.5f, Tween.TransitionType.Sine, Tween.EaseType.InOut);
            animationTween.Start();

            await ToSignal(animationTween, "tween_completed");

            tweetUi.SetGlobalPosition(fillerTweet.GetGlobalPosition() + new Vector2(250, 0));

            animationTween.InterpolateMethod(tweetUi, "set_global_position", tweetUi.GetGlobalPosition(), new Vector2(tweetUi.GetGlobalPosition().x - 250, tweetUi.GetGlobalPosition().y), 0.5f, Tween.TransitionType.Sine,
                Tween.EaseType.InOut);
            animationTween.Start();

            await ToSignal(animationTween, "tween_completed");

            RemoveChild(fillerTweet);
            tmpTweetContainer.RemoveChild(tweetUi);
            AddChild(tweetUi);
            MoveChild(tweetUi, 0);
            RemoveChild(animationTween);
        }
    }
}
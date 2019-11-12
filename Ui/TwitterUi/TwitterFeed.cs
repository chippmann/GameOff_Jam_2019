using System.Collections.Generic;
using GameOff_2019.Data;
using GameOff_2019.Serialization;
using GameOff_2019.Ui.TwitterUi.Dynamic;
using Godot;

namespace GameOff_2019.Ui.TwitterUi {
    public class TwitterFeed : VBoxContainer {
        [Export] private readonly PackedScene tweetPackedScene;
        [Export] private readonly PackedScene tweetWithImagePackedScene;
        [Export] private readonly string pathToTweetsJson = "res://Ui/TwitterUi/DebugTwitterJsonResponse.json";
        [Export] private readonly NodePath slideDownAnimationTweenNodePath = null;
        private Tween slideDownAnimationTween;
        [Export] private readonly NodePath slideUpAnimationTweenNodePath = null;
        private Tween slideUpAnimationTween;

        private List<Tweet> tweets;

        private List<long> shownTweets = new List<long>();

        public override void _Ready() {
            base._Ready();
            ReadTweetsFromDebugJson();
        }

        private int count = 0;
        private float timeElapsed;

        public override void _Process(float delta) {
            base._Process(delta);
            timeElapsed += delta;
            if (timeElapsed >= 2 && count < tweets.Count) {
                timeElapsed = 0;
                AddTweetToFeed(tweets[count]);
                count++;
            }
        }

        private void ReadTweetsFromDebugJson() {
            var file = new File();
            file.Open(pathToTweetsJson, (int) File.ModeFlags.Read);
            var json = file.GetAsText();
            tweets = Serializer.Deserialize<List<Tweet>>(json);
        }

        private void AddTweetToFeed(Tweet tweet) {
            var tweetUi = tweetPackedScene.Instance() as TweetUi;
            shownTweets.Add(tweet.id);
            AddChild(tweetUi);
            MoveChild(tweetUi, 0);
            tweetUi?.Init(tweet);
        }
    }
}
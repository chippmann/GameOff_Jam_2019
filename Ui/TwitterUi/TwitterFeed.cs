using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOff_2019.Data;
using GameOff_2019.EngineUtils;
using GameOff_2019.RoundLogic;
using GameOff_2019.Serialization;
using GameOff_2019.Ui.TwitterUi.Dynamic;
using Godot;

namespace GameOff_2019.Ui.TwitterUi {
    public class TwitterFeed : VBoxContainer {
        [Export] private readonly PackedScene tweetPackedScene;
        [Export] private readonly PackedScene tweetWithImagePackedScene;
        [Export] private readonly PackedScene fillerTweetPackedScene;
        [Export] private readonly PackedScene playerBuffParticlesPackedScene;
        [Export] private readonly PackedScene demonBuffParticlesPackedScene;
        [Export] private readonly string pathToTweetsJson = "res://Ui/TwitterUi/StaticTweets.json";
        [Export] private readonly NodePath tmpTweetContainerNodePath = null;
        private Control tmpTweetContainer;
        [Export] private readonly NodePath playerEnergyNodePath = null;
        private ProgressBar playerEnergy;
        [Export] private readonly NodePath demonEnergyNodePath = null;
        private ProgressBar demonEnergy;


        private List<Tweet> tweets;
        private readonly List<long> shownTweets = new List<long>();
        private readonly Tween buffAnimationTween = new Tween();
        private GameState gameState;
        private Timer tweetTimer;

        public override void _Ready() {
            base._Ready();
            gameState = NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true);
            tmpTweetContainer = GetNode<Control>(tmpTweetContainerNodePath);
            playerEnergy = GetNode<ProgressBar>(playerEnergyNodePath);
            demonEnergy = GetNode<ProgressBar>(demonEnergyNodePath);
            tweetTimer = new Timer {OneShot = true};
            AddChild(tweetTimer);
            AddChild(buffAnimationTween);
            ReadTweetsFromDebugJson();
        }

        private int count = 0;

        public override void _Process(float delta) {
            base._Process(delta);
            if (!tweetTimer.IsStopped()) return;
            if (count >= tweets.Count) {
                count = 0;
            }

            AddTweetToFeed(tweets[count]);
            count++;
            tweetTimer.Start(new Random().Next(10, 40));
        }

        private void ReadTweetsFromDebugJson() {
            var file = new File();
            file.Open(pathToTweetsJson, (int) File.ModeFlags.Read);
            var json = file.GetAsText();
            tweets = Serializer.Deserialize<Statuses>(json).statuses;
            tweets.Sort((tweet1, tweet2) => DateTime.ParseExact(tweet1.created_at, "ddd MMM dd HH:mm:ss zzzz yyyy", null).CompareTo(DateTime.ParseExact(tweet2.created_at, "ddd MMM dd HH:mm:ss zzzz yyyy", null)));
            tweets.Reverse();
        }

        /// <summary>
        /// Don't look at me i'm ugly....<br/>
        /// No seriously because of some weird behaviour regarding the rich text label (also mentioned in some issues) we have to wait with a timer. Uughh :-(
        /// </summary>
        private async void AddTweetToFeed(Tweet tweet) {
            if (!(tweetPackedScene.Instance() is TweetUi tweetUi)) return;
            if (!(fillerTweetPackedScene.Instance() is FillerTweet fillerTweet)) return;
            var animationTween = new Tween();
            var timer = new Timer();
            AddChild(animationTween);
            AddChild(timer);

            shownTweets.Add(tweet.id);

            tmpTweetContainer.AddChild(tweetUi);
            tweetUi.Init(tweet);

            timer.SetOneShot(true);
            timer.Start(0.2f);
            await ToSignal(timer, "timeout");

            tweetUi.SetCustomMinimumSize();

            timer.SetOneShot(true);
            timer.Start(0.2f);
            await ToSignal(timer, "timeout");

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

            fillerTweet.QueueFree();
            tmpTweetContainer.RemoveChild(tweetUi);
            AddChild(tweetUi);
            MoveChild(tweetUi, 0);
            animationTween.QueueFree();
            timer.Start(0.2f);
            await ToSignal(timer, "timeout");
            timer.QueueFree();
            AddBuff(tweet, tweetUi);
        }

        private async void AddBuff(Tweet tweet, TweetUi tweetUi) {
            var isPositive = tweet.entities.hashtags.Any(hashTag => hashTag.text.ToLower() == "climatechange");
            var isNegative = tweet.entities.hashtags.Any(hashTag => hashTag.text.ToLower() == "climatehoax");
            if (isPositive && !isNegative) {
                await AddEnergyAndParticles(tweetUi, playerBuffParticlesPackedScene, playerEnergy.GetGlobalPosition() + playerEnergy.GetSize() / 2, gameState.AddPlayerEnergy);
            }
            else if (!isPositive && isNegative) {
                gameState.negativeTweetCount++;
                await AddEnergyAndParticles(tweetUi, demonBuffParticlesPackedScene, demonEnergy.GetGlobalPosition() + demonEnergy.GetSize() / 2, gameState.AddDemonEnergy);
            }
            else if (isPositive && isNegative) {
                gameState.negativeTweetCount++;
                await AddEnergyAndParticles(tweetUi, playerBuffParticlesPackedScene, playerEnergy.GetGlobalPosition() + playerEnergy.GetSize() / 2, gameState.AddPlayerEnergy);
                await AddEnergyAndParticles(tweetUi, demonBuffParticlesPackedScene, demonEnergy.GetGlobalPosition() + demonEnergy.GetSize() / 2, gameState.AddDemonEnergy);
            }
        }

        private async Task AddEnergyAndParticles(TweetUi tweetUi, PackedScene particlesPackedScene, Vector2 particlesEndPosition, Action<int> addEnergy) {
            if (!(particlesPackedScene.Instance() is Node2D buffParticles)) return;
            GetOwner<Control>().AddChild(buffParticles);
            buffParticles.SetGlobalPosition(tweetUi.GetGlobalPosition() + tweetUi.GetSize() / 2);
            buffAnimationTween.InterpolateMethod(buffParticles, "set_global_position", buffParticles.GetGlobalPosition(), particlesEndPosition, 1f, Tween.TransitionType.Sine, Tween.EaseType.InOut);
            buffAnimationTween.Start();
            await ToSignal(buffAnimationTween, "tween_completed");
            addEnergy.Invoke(GameValues.tweetEnergy);
            var timer = new Timer {OneShot = true};
            AddChild(timer);
            timer.Start(1);
            await ToSignal(timer, "timeout");
            buffParticles.GetNode<Particles2D>("Particles2D").Emitting = false;
            timer.Start(1);
            await ToSignal(timer, "timeout");
            buffParticles.QueueFree();
            timer.QueueFree();
        }
    }
}
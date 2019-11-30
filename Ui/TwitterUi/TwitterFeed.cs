using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOff_2019.Data;
using GameOff_2019.EngineUtils;
using GameOff_2019.RoundLogic;
using GameOff_2019.SoundEngine;
using GameOff_2019.Ui.TwitterUi.Dynamic;
using Godot;

namespace GameOff_2019.Ui.TwitterUi {
    public class TwitterFeed : VBoxContainer {
        [Export] private readonly PackedScene tweetPackedScene = null;
        [Export] private readonly PackedScene tweetWithImagePackedScene = null;
        [Export] private readonly PackedScene fillerTweetPackedScene = null;
        [Export] private readonly PackedScene playerBuffParticlesPackedScene = null;
        [Export] private readonly PackedScene demonBuffParticlesPackedScene = null;

        [Export] private readonly NodePath tmpTweetContainerNodePath = null;
        private Control tmpTweetContainer;
        [Export] private readonly NodePath playerEnergyNodePath = null;
        private ProgressBar playerEnergy;
        [Export] private readonly NodePath demonEnergyNodePath = null;
        private ProgressBar demonEnergy;
        [Export] private readonly NodePath tweetUpdaterNodePath = null;
        private TweetUpdater tweetUpdater;

        [Export] private AudioStreamSample tweetTutorSound = null;
        [Export] private AudioStreamSample tweetNormalSound = null;


        private readonly List<Tweet> tweets = new List<Tweet>();
        private readonly List<Tweet> tutorialTweets = new List<Tweet>();
        private readonly Tween buffAnimationTween = new Tween();
        private GameState gameState;
        private Timer tweetTimer;
        private Timer tutorialTweetTimer;
        private SoundEngineNode soundEngineNode;

        public override void _Ready() {
            base._Ready();
            gameState = NodeGetter.GetFirstNodeInGroup<GameState>(GetTree(), GameConstants.GameStateGroup, true);
            tmpTweetContainer = GetNode<Control>(tmpTweetContainerNodePath);
            playerEnergy = GetNode<ProgressBar>(playerEnergyNodePath);
            demonEnergy = GetNode<ProgressBar>(demonEnergyNodePath);
            tweetUpdater = GetNode<TweetUpdater>(tweetUpdaterNodePath);
            tweetTimer = new Timer {OneShot = true};
            tutorialTweetTimer = new Timer {OneShot = true};
            AddChild(tweetTimer);
            AddChild(tutorialTweetTimer);
            AddChild(buffAnimationTween);
            tweetUpdater.GetTweets(UpdateTweets);
            tutorialTweets.AddRange(tweetUpdater.GetTutorialTweets().statuses);
            soundEngineNode = NodeGetter.GetFirstNodeInGroup<SoundEngineNode>(GetTree(), GameConstants.SoundEngineGroup, true);
        }

        private int count = 0;
        private int tutorialCount = 0;
        private int minTutorialTweetsPassed = 6;

        public override void _Process(float delta) {
            base._Process(delta);
            if (tweetTimer.IsStopped() && tweets.Count > 0 && (gameState.tutorialTweetsCount >= minTutorialTweetsPassed || tutorialTweets.Count <= 0)) {
                if (tweets.Count > 0 && count > tweets.Count / 4 * 3) {
                    tweetUpdater.GetTweets(UpdateTweets);
                }

                if (count >= tweets.Count) {
                    count = 0;
                }

                AddTweetToFeed(tweets[count]);
                count++;
                tweetTimer.Start(new Random().Next(10, 20));
            }

            if (tutorialTweetTimer.IsStopped() && tutorialTweets.Count > 0) {
                if (tutorialCount >= tutorialTweets.Count) {
                    tutorialCount = 0;
                }

                gameState.tutorialTweetsCount++;

                tutorialTweets[tutorialCount].created_at = DateTime.Now.ToString("ddd MMM dd HH:mm:ss zzz yyyy");
                AddTweetToFeed(tutorialTweets[tutorialCount]);
                tutorialCount++;
                tutorialTweetTimer.Start(gameState.tutorialTweetsCount < minTutorialTweetsPassed ? 10 : new Random().Next(10, 20));
            }
        }

        private void UpdateTweets(Statuses statuses) {
            tweets.AddRange(statuses.statuses);
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

            if (tweet.entities.hashtags.Any(hashtag => hashtag.text.ToLower().Equals("tutorial")) || tweet.entities.hashtags.Any(hashtag => hashtag.text.ToLower().Equals("information"))) {
                soundEngineNode.PlaySfx(tweetTutorSound);
            }
            else {
                soundEngineNode.PlaySfx(tweetNormalSound);
            }

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

        public static readonly List<string> positiveHashTags = new List<string>() {"climatechange", "fridayforfuture", "teamtrees"};
        private readonly List<string> negativeHashTags = new List<string>() {"climatehoax", "americafirst"};

        private async void AddBuff(Tweet tweet, TweetUi tweetUi) {
            var isPositive = tweet.entities.hashtags.Any(hashTag => positiveHashTags.Contains(hashTag.text.ToLower()));
            var isNegative = tweet.entities.hashtags.Any(hashTag => negativeHashTags.Contains(hashTag.text.ToLower()));
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
using System;
using System.Globalization;
using System.Linq;
using Godot;
using Planty.Data;

namespace Planty.Ui.TwitterUi.Dynamic {
    public class TweetUi : PanelContainer {
        [Export] private readonly NodePath avatarNodePath = null;
        private TextureRect avatar;
        [Export] private readonly Texture positiveAvatar = null;
        [Export] private readonly Texture negativeAvatar = null;
        [Export] private readonly Texture tutorAvatar = null;
        [Export] private readonly Texture infoAvatar = null;
        [Export] private readonly NodePath nameNodePath = null;
        private Label name;
        [Export] private readonly NodePath displayNameNodePath = null;
        private Label displayName;
        [Export] private readonly NodePath timeSincePostingNodePath = null;
        private Label timeSincePosting;
        [Export] private readonly NodePath textNodePath = null;
        private RichTextLabel text;
        [Export] private readonly NodePath repliesNodePath = null;
        private Label replies;
        [Export] private readonly NodePath retweetsNodePath = null;
        private Label retweets;
        [Export] private readonly NodePath likesNodePath = null;
        private Label likes;

        private Tweet tweet;
        private float lifeTime = 0;

        public override void _Ready() {
            base._Ready();
            avatar = GetNode<TextureRect>(avatarNodePath);
            name = GetNode<Label>(nameNodePath);
            displayName = GetNode<Label>(displayNameNodePath);
            timeSincePosting = GetNode<Label>(timeSincePostingNodePath);
            text = GetNode<RichTextLabel>(textNodePath);
            replies = GetNode<Label>(repliesNodePath);
            retweets = GetNode<Label>(retweetsNodePath);
            likes = GetNode<Label>(likesNodePath);
        }

        public override void _Process(float delta) {
            base._Process(delta);
            lifeTime += delta;
            if (tweet != null) {
                SetTimeSincePosting();
            }

            var displayHeight = GetViewportRect().Size.y;

            if (lifeTime >= 240 || GetGlobalPosition().y > displayHeight) {
                QueueFree();
            }
        }

        public void Init(Tweet tweetToSet) {
            tweet = tweetToSet;
            //TODO: handle image
            name.SetText(tweet.user.name);
            displayName.SetText($"@{tweet.user.screen_name}");
            SetTimeSincePosting();
            var randomReplyCount = new Random().Next(0, tweet.user.followers_count / 50);
            replies.SetText(randomReplyCount > 0 ? randomReplyCount.ToString() : "");
            retweets.SetText(tweet.retweet_count.ToString());
            likes.SetText(tweetToSet.favorite_count.ToString());
            SetAvatar();


            var bbCode = tweet.text;
            foreach (var hashTag in tweet.entities.hashtags) {
                bbCode = bbCode.Insert(bbCode.ToLower().IndexOf("#" + hashTag.text.ToLower(), StringComparison.Ordinal), "[color=#1a95e0]");
                bbCode = bbCode.Insert(bbCode.ToLower().IndexOf("#" + hashTag.text.ToLower(), StringComparison.Ordinal) + ("#" + hashTag.text.ToLower()).Length, "[/color]");
            }

            foreach (var url in tweet.entities.urls) {
                bbCode = bbCode.Insert(bbCode.ToLower().IndexOf(url.url.ToLower(), StringComparison.Ordinal), "[color=#1a95e0][url]");
                bbCode = bbCode.Insert(bbCode.ToLower().IndexOf(url.url.ToLower(), StringComparison.Ordinal) + url.url.ToLower().Length, "[/url][/color]");
            }

            foreach (var mention in tweet.entities.user_mentions) {
                bbCode = bbCode.Insert(bbCode.ToLower().IndexOf(("@" + mention.screen_name).ToLower(), StringComparison.Ordinal), "[color=#1a95e0]");
                bbCode = bbCode.Insert(bbCode.ToLower().IndexOf(("@" + mention.screen_name).ToLower(), StringComparison.Ordinal) + ("@" + mention.screen_name).ToLower().Length, "[/color]");
            }

            text.SetBbcode(bbCode);
        }

        public void SetCustomMinimumSize() {
            text.SetCustomMinimumSize(new Vector2(0, text.GetVScroll().GetMax())); //workaround as rich text label doesn't scale to content -.-
            text.SetSize(new Vector2(213, text.GetVScroll().GetMax()));
            SetSize(new Vector2(250, GetSize().y));
        }

        private void SetTimeSincePosting() {
            //Sat May 04 15:00:33 +0000 2019
            DateTime.TryParseExact(tweet.created_at, "ddd MMM dd HH:mm:ss zzz yyyy", new System.Globalization.CultureInfo("en-US", false), DateTimeStyles.AssumeUniversal, out var tweetTime);
//            var tweetTime = DateTime.ParseExact(tweet.created_at, "ddd MMM dd HH:mm:ss zzz yyyy", new System.Globalization.CultureInfo("en-US", false), DateTimeStyles.AssumeUniversal);
            string timeString;
            var difference = DateTime.Now - tweetTime;

            if (difference.TotalSeconds <= 60) {
                timeString = $"{(int) difference.TotalSeconds}s";
            }
            else if (difference.TotalMinutes <= 60) {
                timeString = $"{(int) difference.TotalMinutes}min";
            }
            else if (difference.TotalHours <= 24) {
                timeString = $"{(int) difference.TotalHours}h";
            }
            else {
                timeString = $"{(int) difference.TotalDays}d";
            }

            timeSincePosting.SetText($"·{timeString}");
        }

        private void SetAvatar() {
            if (tweet.entities.hashtags.Any(hashTag => hashTag.text == "Tutorial")) {
                avatar.SetTexture(tutorAvatar);
            }
            else if (tweet.entities.hashtags.Any(hashTag => hashTag.text == "Information")) {
                avatar.SetTexture(infoAvatar);
            }
            else if (tweet.entities.hashtags.Any(hashTag => TwitterFeed.positiveHashTags.Contains(hashTag.text.ToLower()))) {
                avatar.SetTexture(positiveAvatar);
            }
            else {
                avatar.SetTexture(negativeAvatar);
            }
        }
    }
}
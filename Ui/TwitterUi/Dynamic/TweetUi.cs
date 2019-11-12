using System;
using GameOff_2019.Data;
using Godot;

namespace GameOff_2019.Ui.TwitterUi.Dynamic {
    public class TweetUi : PanelContainer {
        [Export] private readonly NodePath avatarNodePath = null;
        private TextureRect avatar;
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
            if (tweet != null) {
                SetTimeSincePosting();
            }
        }

        public void Init(Tweet tweetToSet) {
            tweet = tweetToSet;
            //TODO: handle image
            name.SetText(tweet.user.name);
            displayName.SetText(tweet.user.screen_name);
            SetTimeSincePosting();
            text.SetText(tweet.text);
            var randomReplyCount = new Random().Next(1, tweet.user.followers_count / 30);
            replies.SetText(randomReplyCount > 0 ? randomReplyCount.ToString() : "");
            retweets.SetText(tweet.retweet_count.ToString());
            likes.SetText(tweetToSet.favorite_count.ToString());

            text.SetCustomMinimumSize(new Vector2(text.GetRect().Size.x, text.GetVScroll().GetMax())); //workaround as rich text label doesn't scale to content -.-
        }

        private void SetTimeSincePosting() {
            //Sat May 04 15:00:33 +0000 2019
            var tweetTime = DateTime.ParseExact(tweet.created_at, "ddd MMMM dd HH:mm:ss zzzz yyyy", null);
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

            timeSincePosting.SetText(timeString);
        }
    }
}
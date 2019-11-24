using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GameOff_2019.Data;
using GameOff_2019.EngineUtils;
using GameOff_2019.Networking;
using GameOff_2019.Serialization;
using Godot;

namespace GameOff_2019.Ui.TwitterUi {
    public class TweetUpdater : Node2D {
        [Export] private readonly NodePath restClientNodePath = null;
        private RestClient restClient;
        [Export] private readonly string pathToTweetsJson = "res://Ui/TwitterUi/StaticTweets.json";
        [Export] private readonly string pathToTutorialTweetsJson = "res://Ui/TwitterUi/TutorialTweets.json";
        private string eTag;
        private Task<RestResponse> currentTask = null;

        public override void _Ready() {
            base._Ready();
            restClient = GetNode<RestClient>(restClientNodePath);
        }


        public async void GetTweets(Action<Statuses> callback) {
            if (currentTask != null && !currentTask.IsCompleted) {
                return;
            }

            try {
                currentTask = restClient.MakeGetRequest(GetTweetApiUrl(), new List<string>(), eTag);
                await currentTask;
                if (currentTask.Result.responseCode == 200) {
                    try {
                        var statuses = Serializer.Deserialize<Statuses>(currentTask.Result.body);
                        eTag = currentTask.Result.headers.First(header => header.StartsWith("Etag")).Split(":")[1].Trim();
                        callback.Invoke(statuses);
                        return;
                    }
                    catch (Exception e) {
                        Logger.Warning($"Couldn't deserialize response from rest call! Error was: {e}!");
                    }
                }
            }
            catch (Exception e) {
                Logger.Warning($"Couldn't make request! Error was: {e}!");
            }


            Logger.Debug($"Couldn't load initial Tweets! Response Code was {currentTask?.Result.responseCode}. Using fallback!");
            callback.Invoke(ReadTweetsFromDebugJson());
        }

        private Statuses ReadTweetsFromDebugJson() {
            var file = new File();
            file.Open(pathToTweetsJson, (int) File.ModeFlags.Read);
            var json = file.GetAsText();
            var statuses = Serializer.Deserialize<Statuses>(json);
            statuses.statuses.Sort((tweet1, tweet2) => {
                DateTime.TryParseExact(tweet1.created_at, "ddd MMM dd HH:mm:ss zzz yyyy", new System.Globalization.CultureInfo("en-US", false), DateTimeStyles.AssumeUniversal, out var dateTime1);
                DateTime.TryParseExact(tweet2.created_at, "ddd MMM dd HH:mm:ss zzz yyyy", new System.Globalization.CultureInfo("en-US", false), DateTimeStyles.AssumeUniversal, out var dateTime2);
                return dateTime1.CompareTo(dateTime2);
            });
            statuses.statuses.Reverse();
            return statuses;
        }

        public Statuses GetTutorialTweets() {
            var file = new File();
            file.Open(pathToTutorialTweetsJson, (int) File.ModeFlags.Read);
            var json = file.GetAsText();
            var statuses = Serializer.Deserialize<Statuses>(json);
            return statuses;
        }

        private string GetTweetApiUrl() {
            var file = new File();
            file.Open(RestClient.CredentialsFilePath, (int) File.ModeFlags.Read);
            var json = file.GetAsText();
            var credentials = Serializer.Deserialize<Credentials>(json);
            return credentials.url;
        }
    }
}
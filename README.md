# GameOff_Jam_2019
Github GameOff Jam 2019 entry developed using the Godot Engine and C#.

## Building
Before building download the `tweetsApiCredentials.json` from our cloud and put it in the `Credentials` folder.   
If you have no access to our cloud, read below.

## Notes for "outsiders" ;-)
Because of the rate limitations of the twitter api we wrote our own api to cache the tweets for each gameSession. The url and credentials for the api are not in this repo! If you build the game yourself, you will only get the tweets in the [StaticTweets.json](Ui/TwitterUi/StaticTweets.json)
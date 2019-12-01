# Planty
Github GameOff Jam 2019 entry developed using the Godot Engine and C#.

## Description
The world is changing...

"ForestGod" tries to save his forest. But he needs YOUR help!
This game is a tribute against all the people that don't care for our world. People that mess around only for their own 
good and not seeing the world for everybody.
The goal of "Planty" is it to save the wold - or at least the little world of our Forest. You can do this with planting 
trees, and being faster than the demon that tries to infest your lovely trees. The demons energy grows with every bad 
tweet and symbolizes the madness that society can grow if they don't start thinking with their own heads.
Give it a try and have fun! :)

Best wishes,
The ForestGod

### About the makers:
"Pure love for code and practically perfect in every way" is what describes us two makers of the game the best. 
We enjoy trying out new technologies and build project with the utopia and dream of a free culture. 
- Someone has to start with it and we want to participate in this utopia!

### More Facts:
* This game was inspired by Princess Mononoke (Studio Ghibli)
* Developed using: C#, Godot, Krita, Synfig Studio, Gimp, Audacity and many more... 
* Everything was done from scratch (apart from four icons)
* Source Code: https://github.com/chippmann/GameOff_Jam_2019

All works are published under the GNU General Public License v3.0

## Building
Before building download the `tweetsApiCredentials.json` from our cloud and put it in the `Credentials` folder.   
If you have no access to our cloud, read below.

## Notes for "outsiders" ;-)
Because of the rate limitations of the twitter api we wrote our own api to cache the tweets for each gameSession.
The url and credentials for the api are not in this repo! If you build the game yourself, you will only get the tweets 
in the [StaticTweets.json](Ui/TwitterUi/StaticTweets.json)

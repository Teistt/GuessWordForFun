# Guess Word For Fun
## Overview
It's a little Find Word game -similar to Charade- I made for fun and to improve my skills on Unity.

The game is divided into 3 rounds and the goal is to make your teammate(s) guess as many words as possible in a given time.

The game starts with a random set of a determined number of cards. All the 3 rounds will be using the same cards.

In each round, one teammate will try to make guess his other teammate(s) as many cards as possible in a fixed time. Once a card is guessed, it's removed from the round. After the end of the timer, if cards remain unguessed for the round, it's to the next team to play. Make sure to alternate the players who have to make guessing within the teams.

The main interest of this project is to be able to adapt several parameters, starting from the number of teams or the number of playing cards, to the drawn words themselves. Indeed, the words are listed in simple .xml files.

Last, do not forget it's a little side project still in work in progress and, and even if I paid special attention to it, bugs are to expect. Moreover, features, UI, and texts are subject to change in the future. I.e., the timer is not configurable yet.

## Add A cards List!
For windows, you can simply add or delete words in the already extisting lists in Assets/StreamingAssets/dictionnaries/ with a text editor.

But you can also add new lists, while for now you will need to download this Project's sources and open them on Unity (last version used is 2020.3.16f1)
For example, to add a Celebrities list:

- Go to Assets/StreamingAssets/dictionnaries/
- Add a "celebs.xml" file with a simple name in lowercase (very important, it can generate bugs with the BetterStreamingAssets plugin)
- File this list as you like. One word per line

![path](https://i.ibb.co/F6FBRFR/image.png)

- On the Unity Editor, add a button in the appropriate UI panel. Despite the sprite, make sure it has the same properties as the "fr" and "en' buttons. Duplicate one of them is cool.

![path](https://i.ibb.co/wKSV1n9/image.png) ![path](https://i.ibb.co/SfC27YQ/image.png)

- Add an On Click action and link the Canvas on it
- Then set TitleGameUI.ChangeLanguage() with "celebs" as parameter

![path](https://i.ibb.co/k8Prr38/image.png)

- While playing, make sure to have less or equal generated cards than words on the list (minimum 10)

Detailed Button config https://i.ibb.co/0ZBpSt3/image.png

/!\ IT MUST BE THE EXACT SAME NAME AS THE NEWLY FILE WITHOUT THE EXTENSION, ALL IN LOWERCASE /!\

## Last Words
I also carried a great interest to do a responsive UI in 1920x1080 while being compatible with Android devices but I could not test all the possibilities, feedback is welcome.

And yes I know there is a lot of French (or English) text. I'm planning to correct this as soon as possible.

# Links!!
All builds are available here: https://teist.itch.io/guesswordforfun
Available:
WebGL version to play in browser

Unity Game Files

Setup for windows

Android apk: it's possible that Android devices display an "alert message" saying that Google does not know me as a developper. Which is true. For now if you are not confident, you can just use the windows version or rebuild the apk from this Unity Project Sources.

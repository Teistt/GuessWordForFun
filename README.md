# Guess Word For Fun
## Overview
It's a little Find Word game I made for fun. I just want to improve on Unity.
The goal is to make his teammates guess as many words as possible in a given time. The first round by describing them, then with a single word, the last by mimicking them.

The main interest of this project is to be able to adjust the number of teams and the number of words, and later the rounds timer (for now only 10 seconds, which is not much).

## Adding a list of words
One other big interest is that all the words are listed in simple xml files, like "dictionnaries" so anybody can go and add some whenever he wants.
The deck system will go through these lists and randomly draw a given number of words. Thanks to that, adding a new "dictionnary" which can be either a language or a specific theme (i.e. Celebrities) is really simple and doesn't even require coding.
But for now, it does require to rebuild the game through Unity. I plan to make this even simpler later.

For example, to add a Celebrities list:
You will need to download this Project's sources and open them on Unity (last version used is 2020.3.16f1)

- Go to Assets/StreamingAssets/dictionnaries/
- Add a "celebs.xml" file with a simple name in lowercase (very important, it can generate bugs with the BetterStreamingAssets plugin)
- File this list as you like. One word per line

![path](https://i.ibb.co/ZNRWynr/image.png)

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
Unity Game Files

Setup for windows

Android apk: it's possible that Android devices display an "alert message" saying that Google does not know me as a developper. Which is true. For now if you are not confident, you can just use the windows version or rebuild the apk from the Unity Project Sources: https://github.com/Teistt/GuessWordForFun

I'm currently working on making a webGL version

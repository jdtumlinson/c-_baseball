# C# Baseball
This repository contains the code for running a C#, text based version of baseball. This was done to learn and understand how C# works compared to other coding languages. This is my first project in the language.

# Playing the Game
After starting the program, you will see that multiple "types" of pitches and hits are loaded. These come from the `Configs` folder, more specifically the files `pitchOdds.json` and `hitOdds.json`. These contain the odds for the different pitches and hits that the user is able to use.

All controls in the game are text based, requiring the user to enter a single character.

At the start of the game, the user will be prompted to pick to be either home or the away team. This dictates if they will pitch or hit first; with the home team hitting first, away team pitching first.

The game will last 9 innings, and then the program will end.

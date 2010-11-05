This is an example using the Conway's Game of Life cucumber steps created by Corey Haines from:
http://github.com/coreyhaines/practice_game_of_life/tree/master/features/ 

In order to run these:

1. Build StorEvil & the examples (just open the solution and ctrl-shift-B).

2. Put storevil in your PATH (by default it will be built in Core/StorEvil/bin/debug).

3. Open a command prompt and navigate to this directory (Examples/ConwaysLife)

4. Run one of the following commands:
  storevil execute
  storevil execute -o ConwaysLife.storevil.output.html

Either of these commands will execute the game of life scenarios in the console. 
The second one will additionally generate an HTML report. You can customized the report by editing the default.spark file in this folder.

The configuration file storevil.config has the settings that drive StorEvil.
You can also run the following commands to get more information:

  storevil help
  storevil help execute

Thanks to Corey Haines for creating these steps.
   


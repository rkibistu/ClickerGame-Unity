# ClickerGame-Unity

link to game (make it fullscreen)
(https://rkibistu.itch.io/clicker-game

This is a prototype of a clicker game. I tried to add skills that are based on the timing press of some buttons to add a plus to classic clicker games. I stopped after adding the first skill (uppercut) but may add more later.

I am gonna try to add a short description of the most important systems of the game.
## Player
    PlayerController script takes care of player movement and input. 
      Movement is very simple: 
    -	Run to the right
    -	If you encounter an enemy -> stop
    -	Attack until the enemy is dead (can click to speed up the attack)
    You can press Q, W, E to activate skills that you gain by leveling up.
    PlayerStats script keeps the current stats of the player (health, mana, exp) and implements methods for changing them.
    Some events change the graphical view of the stats on changes in the code.
## Skills
	  SkillController script takes care of cooldowns, adding and changing skills.

  	The UpperCut is a skill that throws the enemy into the air.
    You can use it multiple times if you hit it at the right moment and every time will blow the enemy further.
## Camera
  	The parallax script creates the parallax effect of the background that is composed of 4 layers.
  	SecondCamera script splits the screen into 2 parts and follows the enemy while it is thrown into the air.

## Enemy
  	The EnemySpawner implements methods to spawn the enemy.
  	The EnemyController calls methods to spawn the enemy, move the enemy or start the second camera movement when needed.

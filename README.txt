Created by Geordan Krahn for the Unity Game Engine.

These scripts should allow simple player motion, attack and hit detection
  as well as simple enemy Ai and off screen spawning.
  The following instructions are a guide to using these scripts.
  It is assumed you have enough knowledge to figure out how to use these scripts in your own project.
  you may adapt and improve on them in your own project
  If you decide to use or modify this code please give me credit, A little recognition for my work is all I ask.

Set up your player
Attach the Player.cs, PlayerAttack.cs and PlayerControl.cs to your player gameobject
  create a child object of the player call it Activation zone and attach the SpawnActivation.cs to it
  attach a box collider 2d with the is trigger property checked
  This this to activate the spawners so they can begin spawning enemies off screen
  
  Create another child of the player called attack box(one for each type of attack)
  attach the PlayerHitBox.cs to each
  attach another empty child gameobject of the player for each attack box and call it attack position
  Create 2 child objects of each of the attack boxs(parent should be the attack box of the player)
  call them attack start position and attack rotation point
  
  Player
    ActivationZone
    AttackPosition(for each)
    AttackBox(for each)
      AttackStartPosition
      AttackRotationPoint
  
Set up your enemies -- enemies can be set up using the same hit detection as player, or just a simple circle cast --
  just set the enemy up as you would the player.

the following is for setting up using hit box for attack hit detection -- for circle cast, you do not need to attach EnemyHitBox.cs
Attach the GroundEnemy.cs and GroundEnemyAi.cs to your enemy gameobject
  Create a child of the enemy and call it attack box(for each type of attack)
  Attach the enemy EnemyHitBox.cs to the enemy gameobject
  attach another empty child gameobject for each attack box and call it attack position
  Create 2 child objects of each of the attack boxs(parent should be the attack box of the enemy)
  call them attack start position and attack rotation point
  
The following is setting up the enemy with circle casts
  create child object for attack position
  create child object for line of sight and attach a cicle colliders 2d with the is trigger property checked
  this is the enemies player detection zone, when the player invades the colliders, the enemy will pursue the player and attack
  
Setup your camera
Attach the CameraFocus.cs to your main camera
  Set the player as the gameobject to follow
 
Set up your spawners
Create an empty gameobject and give it a box collider 2d with the is trigger property checked
  Place it where you want to spawn your enemies and attach the EnemySpawner.cs


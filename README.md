# Bad Piggies :: Rebooted Edition
A Community-driven Modification of the game Bad Piggies!
Thanks to Miuna for all the prior work from the mod called BPLE, we really hope you had more time to spare for your project.

## Features:
###### Work in Progress, TONS of features to mention...

## Known Issues:
+ Part will bug when connected to π/3 triangle (most likely physics issue, meshcollider maybe?)
+ Triangles prevents rotation
+ Script Engine fails to load assembly on android
+ Command /fill & /setpart will temporaryily fail grid menu and sometimes disable crafting entirely
+ /gamerule set terrainscale does not work immediately (&& while it works it potentially removing your contraption?)
+ Neural Part slot is overflowed with triangles
##### Possible Improvements
+ Add AddPart method instead of using new fields in RECommandInterface.cs
+ Add metadata to savefile, currently // comments are available. and # will be used for meta settings (maybe add a corresponding command to do this)

## Changes:
###### Work in Progress, too, 
#### Quality Of Life
+ Mute music button (still weird icon)
+ Mute starbox sound
+ Remove (almost) zoom limits
+ C# Runtime Compilation Engine and it's User Interface
+ Command System
+ Job system used with Burst @ entitylight physics
+ More Types of ExplodingGrapplingHookProjectile, More expandability 
#### Changes that doesn't effect gameplay
+ Change version selector background
+ Change Title Image
#### Existing part changes
+ Higher Jet Engine thrust limits (6000)
+ FULLY COMPLETED Part HP system, with selectable modes (mode 0 is off)
+ Explosions, Collsions now deals damage to parts
+ Part is destroyed and removed when hp<0 in mode 1, turned into debris in mode 2
#### New Parts
+ Alien Wooden Frame, much flexible with engine power
+ Alien Santa Spring (3*mass, high connect strength)
+ Alien Rapid Gun (Rapidly shoot bullets without cooldown with simulated AP, use circuits to spam)
+ Alien Metal TNT (Much heavier(75kg) and stronger), it also has engine power
+ Alien Rapid Gun Extended (Alien Gatling Gun), just like Alien Rapid Gun(without auto spam), also new bullets for that(Stackable, Uses Gravity,  Big Mass)
+ Oscillator (Produces periodic signals when activated)
+ Healing Light (Heals all parts within 5-meter-radius at a rate of *300*hp/s) (60 before)
+ Lightning Cannon(comes with lightning bullet)
+ Force Field Light **-RESURRECTION-** (repels/sucks objects within 5 meters radius with a force inverse proportional to [distance^2+0.2f])
+ (Still WIP/BUG, prefab named "MovableLight") Machine piston **-RESURRECTION-**, well, Pistons!
+ Triangle Frames
+ Metal Slab Frame
+ Never Gonna Give You up
+ Never Gonna Let You Down

## Legal Notice:
**This mod is not affiliated with Rovio in any way. Please note that the game Bad Piggies is developed by Rovio. Most of the unmodified source codes and resources belongs to them.**

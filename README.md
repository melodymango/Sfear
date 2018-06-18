# Sfear
UCI ICS 168 Multiplayer Mobile Game Prototype by:

Kevin Chase, Melody Chung, Amanda Osterkamp
## Description
- 3D top-down real time multiplayer competitive mobile game
- Players must avoid having a deadly curse when time is up
## Download Latest Build:
[to be made]

## Details
- Game takes place on a sphere. (Think Super Mario Galaxy where you can traverse the entire mini-planet)
- 2 to 4 players
- All players start the round in one spot
- One player is randomly assigned to be cursed
- Only one player is cursed at any time
- All other players must avoid being cursed, or pass on their curse before time is up
- The player who has held the curse for the least amount of time wins
## Core Mechanics
- **Movement**: On a mobile device, movement would be achieved by swiping. Movement is physics based - players don’t stop immediately, but slide to a stop. Swiping more increases the player’s speed and momentum, but there is a speed cap. the On our current build, we tried to emulate swiping controls by requiring the player to click, hold, and flick the mouse.
- **The Curse**: The cursed player passes their curse people by running into another player. Only one player can be cursed at a time, so the game basically plays like hot potato. Whenever a player passes off their curse to someone else, they gain temporary invulnerability to prevent the other player from “tagging” them back.
- **Invisibility**: The cursed player can increase their chances of tagging people by becoming invisible for a short amount of time. There is a cooldown so the cursed player can’t be invisible indefinitely. On a mobile device invisibility would be achieved by double-tapping. On our build, invisibility can be activated by double clicking.
## Features to be Implemented
- Expanded multiplayer options - matchmaking and private match with friends
- Giving the cursed player a transparent mesh render while they are invisible to improve visibility (they would still be completely invisible to everyone else)
- Some sort of way for non-cursed players to counter the cursed player
  - Sprint button to increase speed
- More environmental obstacles:
  - Terrain with different physics effects (icy = slippery, muddy = slow, etc.)




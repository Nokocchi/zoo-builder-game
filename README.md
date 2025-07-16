# Zoo Builder

Explore the world, gather materials, collect animals, grow food, and build your zoo.

# TODO

The tasks below are "minimum implementations" of basic, core features in the game. The idea is that if you can
change a few settings, then you can easily add more, and if you can create a simple quest line, you can easily
add more.

Actual storyline details, specific features or game mechanics will be added afterwards.

## Player

- ✅ Player character, controllable with WASD + Space, can jump
- ✅ Character animation when moving
- ✅ 3D camera movable with mouse
- ✅ Holding "forward" (W) always goes in the direction the camera is facing
- Some kind of cursor/highlighted grid square (maybe just when holding specific tools?)

## Menu
- **Full screen inventory with all your items**
  - ✅ Opening stops character movements and shows your mouse
  - When clicking an item, the item is picked up and follows the cursor.
  - When holding an item and clicking another slot, move item to this spot. Swap items if necessary
  - When holding an item and clicking either Q or outside of inventory, drop in overworld
  - Right click item to split stack in half
- **Simple Hotbar**
  - ✅ Show some of your most used items
  - ✅ One item in the hotbar is always highlighted - can be switched by scrolling with mouse
- **Collections view**
  - Keep track of which items the player has picked up
  - Show ? if never seen, black box is seen but not picked up, image if picked up
- **Achievements view**
  - Keep track of game stats, like number of items picked up, meters walked
  - Show achievement here if some number is reached
- **Settings view**
  - ✅ Change mouse sensitivity
  - ✅ Change up/down direction of mouse
  - ✅ Hide minimap
  - Save and load game buttons
  - Adjust audio
  - ✅ Hotbar scrolling direction
- **Quest view**
  - Show 3 tabs. Active quests, completed quests, all quests
  - Completed quests greyed out with checkmark icon
- **Tabs**
- Put all these views into the same UI, but in different tabs.

## HUD
  - Minimap
    - ✅ Minimap showing the player from above
    - Minimap's up direction should always be where the camera is facing
  - Digital clock

## Overworld entities
- A chest with infinite storage, scroll
- NPC
  - Random walking/movement
  - Animated
  - Can be interacted with when nearby
  - Dialogue when talked to, including text box at the bottom of the screen
- ✅ Overworld items that get picked up when touched
  - ✅ Image always facing the player
  - ✅ Move items towards player if nearby
  - Play sound when picked up
  - Stack items on the ground if multiple are in close proximity

## Save / Load
- Save achievements
- ✅ Save settings
- Save character customization
- Save quest progression
- Save inventory
- Save current location(?)

## Storyline, gameplay
- Simple quest system. Activated by talking to an NPC. Pick up apple and give to NPC. Next quest, pick up 2 oranges and give to NPC.
- Time system. Clock starts at 06:00 and ticks until 24:00 when it resets.

## Intro
- **Menu**
  - New Game button
  - Continue button
  - Settings button (Should open same settings as in inventory)
  - Background music
  - Animated background
- **Character creation screen**
  - Simple naked character, with a few different hairs and clothes to pick between.

## Rendering, visuals
- Sun moves through the sky as time passes
- Moon moves through the sky as time passes (night)
- Warmer light during morning and evening
- Shadows based on sun position

# Known bugs
- Hotbar selection keeps disappearing, like when changing scroll direction or hiding minimap
- Hotbar selection changes when opening inventory with Tab
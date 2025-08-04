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
  - ✅ When clicking an item, the item is picked up and follows the cursor.
  - ✅ When holding an item and clicking another slot, move item to this spot. Swap items if necessary
  - ✅ When holding an item and clicking Q, drop in overworld
  - ✅ When holding an item and clicking outside inventory, drop whole stack in overworld
  - ✅ When not inventory not open and item is selected in hotbar and you press Q, drop one from stack in overworld
  - Right click item to split stack in half
  - Reuse the same script for hotbar, inventory and chests (or at least avoid duplication when possible)
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
  - ✅ Adjust audio
  - ✅ Hotbar scrolling direction
  - Only save when save button is clicked
- **Quest view**
  - Show 3 tabs. Active quests, completed quests, all quests
  - Completed quests greyed out with checkmark icon
- **Tabs**
  - Put all these views into the same UI, but in different tabs.

## HUD
  - ✅ Minimap
	- ✅ Minimap showing the player from above
	- ✅ Minimap's up direction should always be where the camera is facing
  - ✅ Digital clock

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
  - ✅ Combine into one stack if multiple of the same item are near each other
  - Play sound when picked up

## Save / Loadq
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
  - ✅ New Game button
  - Continue button
  - Settings button (Should open same settings as in inventory)
  - ✅ Background music
  - Animated background
- **Character creation screen**
  - Simple naked character, with a few different hairs and clothes to pick between.

## Rendering, visuals
- Sun moves through the sky as time passes
- Moon moves through the sky as time passes (night)
- Warmer light during morning and evening
- Shadows based on sun position

---

# Known bugs
- Hotbar selection keeps disappearing, like when changing scroll direction or hiding minimap
- Held item, Q until stack is empty - held item doesn't disappear from UI
- Two items in inventory, hold second item and Q until stack is empty. First item now disappears from inventory 

# Things to fix:
- Currently, the logic in the hotbar is duplicated to the inventory. This should not be necessary. Simplify with inheritance or just use a single script file
- The inventory should be a resource so it can be saved and loaded
- When items collide with static objects, they can be picked up. The only static object at the moment is the floor, but in the future, there will be walls.

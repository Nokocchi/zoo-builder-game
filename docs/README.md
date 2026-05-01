# Zoo Builder

Explore the world, gather materials, collect animals, grow food, and build your zoo.

# TODO

The tasks below are "minimum implementations" of basic, core features in the game. The idea is that if you can
change a few settings, then you can easily add more, and if you can create a simple quest line, you can easily
add more.

Actual storyline details, specific features or game mechanics will be added afterwards.

Temporary order of implementation..

- X Player state machine (IN_AIR, WALKING only states at the moment, so not very easy or useful)
- Overworld chest
  - NPCs
    - Dialogue system
- Save which overworld items have been picked up
  - Quests (Item only loads if quest is active, Pick up item, go to area, item is removed from inventory, quest is completed in save file)
- Save slots and save file history
  - Settings menu for saving and loading
  - Character customizations when starting new game
    - Save character customization
- Tests

## Player

- ✅ Player character, controllable with WASD + Space, can jump
- ✅ Character animation when moving
- ✅ 3D camera movable with mouse
- ✅ Holding "forward" (W) always goes in the direction the camera is facing
- State machine

## Menu
- **Full screen inventory with all your items**
  - ✅ Opening stops character movements and shows your mouse
  - ✅ When clicking an item, the item is picked up and follows the cursor.
  - ✅ When holding an item and clicking another slot, move item to this spot. Swap items if necessary
  - ✅ When holding an item and clicking Q, drop in overworld
  - ✅ When holding an item and clicking outside inventory, drop whole stack in overworld
  - ✅ When not inventory not open and item is selected in hotbar and you press Q, drop one from stack in overworld
  - ✅ Right click item to split stack in half
  - ✅ Hold selected hotbar item in hand in overworld
  - ✅ Item hover popup with information about the item
  - ✅ Reuse the same script for hotbar, inventory and chests (or at least avoid duplication when possible)
  - ❌ Scrollable inventory with square item slots (They are currently only square if you have enough of them..)
- **Simple Hotbar**
  - ✅ Show some of your most used items
  - ✅ One item in the hotbar is always highlighted - can be switched by scrolling with mouse
- **Collections view**
  - Keep track of which items the player has picked up
  - If they player has picked up an overworld item, it should not load in the overworld anymore in that save file
  - Show ? if never seen, black box is seen but not picked up, image if picked up
- **Achievements view**
  - ❌ Keep track of game stats, like number of items picked up, meters walked
  - ❌ Show achievement here if some number is reached
- **Settings view**
  - ✅ Change mouse sensitivity
  - ✅ Change up/down direction of mouse
  - ✅ Hide minimap
  - Save and load game buttons
  - Show "last saved x minutes ago", and each save should say "22 minutes ago" etc.
  - ✅ Adjust audio
  - ✅ Hotbar scrolling direction
  - ✅ Only save when save button is clicked
  - ✅ Allow players to remap input
  - Various graphics settings
- **Quest view**
  - Show 3 tabs. Active quests, completed quests, all quests
  - Completed quests greyed out with checkmark icon
- **Tabs**
  - ✅ Put all these views into the same UI, but in different tabs.
- **Localization**
  - ✅ It should be possible to pick from a list of supported languages and all text is instantly in that language
- **Themes**
  - ✅ Consistent theme across all menus
- **Input**
  - Support for gamepad

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
  - ✅ Play sound when picked up

## Save / Load
- ✅ Save achievements
- ✅ Save settings
- Save character customization
- Save quest progression
- ✅ Save inventory
- ✅ Save current location(?)
- Multiple save files, selectable from Continue button on main menu

## Storyline, gameplay
- Simple quest system. Activated by talking to an NPC. Pick up apple and give to NPC. Next quest, pick up 2 oranges and give to NPC.

## Intro
- **Menu**
  - ✅ New Game button
  - Continue button
  - ✅ Settings button (Should open same settings as in inventory)
  - ✅ Background music
- **Character creation screen**
  - Simple naked character, with a few different hairs and clothes to pick between.

## Rendering, visuals
- ✅ Sun moves through the sky as time passes
- ✅ Moon moves through the sky as time passes (night)
- ✅ Warmer light during morning and evening
- ✅ Shadows based on sun position
- Hook up the animation time/speed to match the in-game time. Especially important once the time of day is saved when the game saves.

## Others
- ✅ Autoload EventBus using generic C# events
- Proper tests
- Simple Multiplayer (shared quests, steam invite, separate inventories, chests that update in real time without race conditions?)

---

# Known bugs

# Things to fix:
- When items collide with static objects, they can be picked up. The only static object at the moment is the floor, but in the future, there will be walls.
- Sun/moon teleport to the other side at the end of a day/night animation cycle, and this causes the light in the scene to blink.
- Game crashes if Steam is not running.. How to handle that? 

# Things to figure out:
- When starting a new game, the player would expect all their achievements and stats to be reset for that save file. Maybe you can have multiple save files with different progress. How does that work with Steam stats and achievements which seem to be "global" on the user's steam account?

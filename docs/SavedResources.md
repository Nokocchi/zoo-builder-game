# Save games, settings, saved resources

All data needed for a save file should be stored within one single Resource. It could be called GameData.

This GameData should be all that is necessary to store a current game, and to load an exact copy at a later time.

Additionally, there should be a Settings file which is reused between save files. 

This means that we only need 2 resources to be stored and loaded. 

I might still want to have separate Resource files for things like inventory, just so that I can easily
mess around with them during testing. I.e, a few different .tres files that hold different kinds of inventories, 
and these can then easily be switched out in the editor when trying different things.

But I do not want to save such an inventory resource file directly and explicitly. Instead, it should 
simply be stored on the parent GameData resource file, and it will automatically be saved when the
GameData resource file is saved.
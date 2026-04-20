using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;
using ZooBuilder.globals.saveable;
using ZooBuilder.ui.settings;

public class GlobalDataSingleton
{
    // TODO: This can probably be cleaned up quite a bit
    // TODO: Generate tabs for each category
    // TODO: Warning when closing settings menu with unchanged settings
    // TODO: Each settings category should have a defaults list, and a way to restore defaults and save etc. - maybe a common interface?
    // TODO: If the player picks a key that is already used, show what it is used for and give the option to cancel or swap. 
    
    public static readonly string BOOL_TYPE_NAME = nameof(Boolean);
    public static readonly string FLOAT_TYPE_NAME = nameof(Single);
    public static readonly string STRING_TYPE_NAME = nameof(String);
    public static readonly string INPUT_EVENT_TYPE_NAME = "InputEvent";
    
    public static readonly string SETTINGS_CATEGORY_GAMEPLAY = "Gameplay";
    public static readonly string SETTINGS_CATEGORY_OTHER = "Other";
    public static readonly string SETTINGS_CATEGORY_INPUT = "Input";
    
    public static readonly string SETTINGS_LOCATION = "user://settings_3.json";
    
    public static readonly string SETTINGS_INPUT_GROUP_NAME = "settings_input_group";
    
    public static readonly string KEY_MOUSE_SENSITIVITY = "SETTINGS_MOUSE_SENSITIVITY";
    public static readonly string KEY_BACKGROUND_MUSIC_AUDIO_VOLUME = "SETTINGS_AUDIO_BG_VOL";
    public static readonly string KEY_MOUSE_Y_FLIPPED = "SETTINGS_FLIP_MOUSE_UP_DOWN";
    public static readonly string KEY_HOTBAR_SCROLL_DIRECTION_FLIPPED = "SETTINGS_FLIP_HOTBAR_SCROLL_DIRECTION";
    public static readonly string KEY_HIDE_MINIMAP = "SETTINGS_HIDE_MINIMAP";
    public static readonly string KEY_NORTH_FACING_MINIMAP = "SETTINGS_NORTH_FACING_MINIMAP";
    public static readonly string KEY_SELECTED_LOCALE = "SelectedLocale";
    
    public static readonly string ACTION_MOVE_RIGHT = "move_right";
    public static readonly string ACTION_MOVE_LEFT = "move_left";
    public static readonly string ACTION_MOVE_FORWARD = "move_forward";
    public static readonly string ACTION_MOVE_BACK = "move_back";
    public static readonly string ACTION_JUMP = "jump";
    public static readonly string ACTION_RUN = "run";
    public static readonly string ACTION_OPEN_SETTINGS = "open_settings";
    public static readonly string ACTION_OPEN_ACHIEVEMENTS = "open_achievements";
    public static readonly string ACTION_OPEN_INVENTORY = "open_inventory";
    public static readonly string ACTION_TOSS_SINGLE_ITEM = "toss_single_item";

    // ACCESSORS
    
    public static GlobalData Instance { get; private set; }
    
    public static bool MouseYFlipped => Instance.Get<bool>(KEY_MOUSE_Y_FLIPPED);
    public static bool HotbarScrollDirectionFlipped => Instance.Get<bool>(KEY_HOTBAR_SCROLL_DIRECTION_FLIPPED);
    public static bool HideMinimap => Instance.Get<bool>(KEY_HIDE_MINIMAP);
    public static bool NorthFacingMinimap => Instance.Get<bool>(KEY_NORTH_FACING_MINIMAP);
    public static float MouseSensitivity => Instance.Get<float>(KEY_MOUSE_SENSITIVITY);
    public static float BackgroundMusicAudioVolume => Instance.Get<float>(KEY_BACKGROUND_MUSIC_AUDIO_VOLUME);
    public static string SelectedLocale => Instance.Get<string>(KEY_SELECTED_LOCALE);

    public static void LoadSettingsFromDisk()
    {
        Instance = GlobalData.LoadFromDisk();
    }

    public static void SaveToDisk()
    {
        Instance.SaveToDisk();
    }
}
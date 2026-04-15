using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;
using ZooBuilder.globals.saveable;
using ZooBuilder.ui.settings;

public class GlobalDataSingleton
{
    
    // TODO: Can we move a lot of the JSON conversion (BOOL_TYPE_NAME and SetDataFromDto() and AsJsonDto()) into the GlobalDataJsonDTO, or at least a helper method?
    // TODO: Make it nicer to work with custom setting inputs like language selector
    // TODO: Fix onSaveCallback. Doesn't seem to work for audio and language
    // TOdo: In settings.cs, don't create new Settings, but just update the existing ones.

    public static readonly string BOOL_TYPE_NAME = nameof(Boolean);
    public static readonly string FLOAT_TYPE_NAME = nameof(Single);
    public static readonly string STRING_TYPE_NAME = nameof(String);
    
    public static readonly string SETTINGS_LOCATION = "user://settings_3.json";
    
    public static readonly string SETTINGS_INPUT_GROUP_NAME = "settings_input_group";
    
    public static readonly string KEY_MOUSE_SENSITIVITY = "SETTINGS_MOUSE_SENSITIVITY";
    public static readonly string KEY_BACKGROUND_MUSIC_AUDIO_VOLUME = "SETTINGS_AUDIO_BG_VOL";
    public static readonly string KEY_MOUSE_Y_FLIPPED = "SETTINGS_FLIP_MOUSE_UP_DOWN";
    public static readonly string KEY_HOTBAR_SCROLL_DIRECTION_FLIPPED = "SETTINGS_FLIP_HOTBAR_SCROLL_DIRECTION";
    public static readonly string KEY_HIDE_MINIMAP = "SETTINGS_HIDE_MINIMAP";
    public static readonly string KEY_NORTH_FACING_MINIMAP = "SETTINGS_NORTH_FACING_MINIMAP";
    public static readonly string KEY_SELECTED_LOCALE = "SelectedLocale";

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

    public static void Save(List<ISetting> newSettingsToSave)
    {
        Instance.OverrideSettings(newSettingsToSave);
        foreach (ISetting setting in newSettingsToSave)
        {
            setting.executeOnSaveCallback();
        }
        Instance.SaveToDisk();
    }
}
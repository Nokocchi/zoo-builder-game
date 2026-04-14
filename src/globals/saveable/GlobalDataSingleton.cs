using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;
using ZooBuilder.globals.saveable;

public class GlobalDataSingleton
{

    public static readonly string BOOL_TYPE_NAME = nameof(Boolean);
    public static readonly string FLOAT_TYPE_NAME = nameof(Single);
    public static readonly string STRING_TYPE_NAME = nameof(String);
    
    public static readonly string SETTINGS_LOCATION = "user://settings.json";
    
    public static readonly string SETTINGS_INPUT_GROUP_NAME = "settings_input_group";
    
    public static readonly string KEY_MOUSE_SENSITIVITY = "SETTINGS_MOUSE_SENSITIVITY";
    public static readonly string KEY_BACKGROUND_MUSIC_AUDIO_VOLUME = "SETTINGS_AUDIO_BG_VOL";
    public static readonly string KEY_MOUSE_Y_FLIPPED = "SETTINGS_FLIP_MOUSE_UP_DOWN";
    public static readonly string KEY_HOTBAR_SCROLL_DIRECTION_FLIPPED = "SETTINGS_FLIP_HOTBAR_SCROLL_DIRECTION";
    public static readonly string KEY_HIDE_MINIMAP = "SETTINGS_HIDE_MINIMAP";
    public static readonly string KEY_NORTH_FACING_MINIMAP = "SETTINGS_NORTH_FACING_MINIMAP";
    public static readonly string KEY_SELECTED_LOCALE = "SelectedLocale";
    
    // These are defaults and not editable by the user, so don't store in the GlobalData DTO
    public static readonly Dictionary<string, (float, float)> FloatSettingMinMax = new()
    {
        [KEY_MOUSE_SENSITIVITY] = (0, 100),
        [KEY_BACKGROUND_MUSIC_AUDIO_VOLUME] = (0, 100)
    };

    // ACCESSORS
    
    public static GlobalData Instance { get; private set; }
    
    public static bool MouseYFlipped => Instance.Get<bool>(KEY_MOUSE_Y_FLIPPED);
    public static bool HotbarScrollDirectionFlipped => Instance.Get<bool>(KEY_HOTBAR_SCROLL_DIRECTION_FLIPPED);
    public static bool HideMinimap => Instance.Get<bool>(KEY_HIDE_MINIMAP);
    public static bool NorthFacingMinimap => Instance.Get<bool>(KEY_NORTH_FACING_MINIMAP);
    public static float MouseSensitivity => Instance.Get<float>(KEY_MOUSE_SENSITIVITY);
    public static float BackgroundMusicAudioVolume => Instance.Get<float>(KEY_BACKGROUND_MUSIC_AUDIO_VOLUME);
    public static string SelectedLocale => Instance.Get<string>(KEY_SELECTED_LOCALE);

    public static void LoadFromDisk()
    {
        using FileAccess file = FileAccess.Open(SETTINGS_LOCATION, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            Error openError = FileAccess.GetOpenError();
            if (openError is Error.DoesNotExist or Error.FileNotFound)
            {
                // TODO: Handle other error cases
                Instance = new GlobalData();
                return;
            }

            GD.Print(openError);
        }

        string json = file.GetAsText();
        Instance = JsonSerializer.Deserialize<GlobalData>(json);
    }

    public static void Save(GlobalData dataToStore)
    {
        // TODO: This method is doing too many different things. Could it just call a bunch of callback methods instead?
        Instance = dataToStore;
        string serializedJson = JsonSerializer.Serialize(dataToStore);
        using FileAccess file = FileAccess.Open(SETTINGS_LOCATION, FileAccess.ModeFlags.Write);
        file.StoreString(serializedJson);
        TranslationServer.SetLocale(SelectedLocale);
        int audioBusIndexBgMusic = AudioServer.GetBusIndex("BgMusic");
        AudioServer.SetBusVolumeLinear(audioBusIndexBgMusic,  BackgroundMusicAudioVolume / 100);
    }
}
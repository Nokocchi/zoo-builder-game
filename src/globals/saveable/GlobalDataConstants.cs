namespace ZooBuilder.globals.saveable;

public class GlobalDataConstants
{
    public static readonly string KEY_MOUSE_SENSITIVITY = "SETTINGS_MOUSE_SENSITIVITY";
    public static readonly string KEY_BACKGROUND_MUSIC_AUDIO_VOLUME = "SETTINGS_AUDIO_BG_VOL";
    public static readonly string KEY_MOUSE_Y_FLIPPED = "SETTINGS_FLIP_MOUSE_UP_DOWN";
    public static readonly string KEY_HOTBAR_SCROLL_DIRECTION_FLIPPED = "SETTINGS_FLIP_HOTBAR_SCROLL_DIRECTION";
    public static readonly string KEY_HIDE_MINIMAP = "SETTINGS_HIDE_MINIMAP";
    public static readonly string KEY_NORTH_FACING_MINIMAP = "SETTINGS_NORTH_FACING_MINIMAP";
    public static readonly string KEY_SELECTED_LOCALE = "SelectedLocale";
    
    public static readonly (float, float) MouseSensitivityMinMax = (0, 100);
    public static readonly (float, float) BackgroundMusicMinMax = (0, 100);

}
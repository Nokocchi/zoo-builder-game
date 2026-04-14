namespace ZooBuilder.ui.settings;

public interface ISettingInput
{
    string SettingsKey { get; }
    object GetValue();
}
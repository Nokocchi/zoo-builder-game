namespace ZooBuilder.ui.settings;

public interface ISettingInput
{
    string SettingsKey { get; }
    ISetting GetAsSetting();
}
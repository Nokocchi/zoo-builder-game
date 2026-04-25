namespace ZooBuilder.ui.settings;

public interface ISettingInput
{
    void SaveInputStateToGlobalSetting();
    void RestoreDefault();
}
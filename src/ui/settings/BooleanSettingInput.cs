using Godot;
using System;
using ZooBuilder.ui.settings;

public partial class BooleanSettingInput : PanelContainer, ISettingInput
{
    private static readonly PackedScene BooleanSettingInputScene = GD.Load<PackedScene>("res://src/ui/settings/boolean_setting_input.tscn");
    private CheckBox _checkBox;
    private Label _label;

    private Setting<bool> _setting;

    public override void _Ready()
    {
        _checkBox = GetNode<CheckBox>("%CheckBox");
        _label = GetNode<Label>("%Label");
        _checkBox.SetPressed(_setting.GetValue());
        _label.Text = _setting.Key;
    }

    public static BooleanSettingInput CreateWithValue(Setting<bool> setting)
    {
        BooleanSettingInput input = BooleanSettingInputScene.Instantiate<BooleanSettingInput>();
        input._setting = setting;
        return input;
    }

    public void SaveInputStateToGlobalSetting()
    {
        _setting.SaveNewValue(_checkBox.ButtonPressed);
    }

    public void RestoreDefault()
    {
        bool defaultValue = GlobalDataSingleton.Defaults.Get<bool>(_setting.Key);
        _setting.SaveNewValue(defaultValue);
    }
}
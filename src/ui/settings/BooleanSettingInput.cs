using Godot;
using System;
using ZooBuilder.ui.settings;

public partial class BooleanSettingInput : PanelContainer, ISettingInput
{
	
	private static readonly PackedScene BooleanSettingInputScene = GD.Load<PackedScene>("res://src/ui/settings/boolean_setting_input.tscn");
	private CheckBox _checkBox;
	private Label _label;
	
	public object GetValue() => _checkBox.ButtonPressed;
	public string SettingsKey { get; private set; }
	private bool _initialValue;

	public override void _Ready()
	{
		AddToGroup(GlobalDataSingleton.SETTINGS_INPUT_GROUP_NAME);
		_checkBox = GetNode<CheckBox>("%CheckBox");
		_label = GetNode<Label>("%Label");
		_checkBox.SetPressed(_initialValue);
		_label.Text = SettingsKey;
	}

	public static BooleanSettingInput CreateWithValue(string settingsKey, bool value)
	{
		BooleanSettingInput input = BooleanSettingInputScene.Instantiate<BooleanSettingInput>();
		input._initialValue = value;
		input.SettingsKey = settingsKey;
		return input;
	}
}

using Godot;
using System;

public partial class BooleanSettingInput : PanelContainer
{
	[Signal]
	public delegate void ValueChangedEventHandler(bool isPressed);
	
	private static readonly PackedScene BooleanSettingInputScene = GD.Load<PackedScene>("res://src/ui/settings/boolean_setting_input.tscn");
	private CheckBox _checkBox;
	private Label _label;
	
	private bool _value;
	private string _labelText;

	public override void _Ready()
	{
		_checkBox = GetNode<CheckBox>("%CheckBox");
		_label = GetNode<Label>("%Label");
		_checkBox.SetPressed(_value);
		_label.Text = _labelText;
		EmitSignal(SignalName.ValueChanged, true);
		_checkBox.Toggled += (isPressed) => EmitSignal(SignalName.ValueChanged, isPressed);
	}

	public static BooleanSettingInput CreateWithValue(string label, bool value)
	{
		BooleanSettingInput input = BooleanSettingInputScene.Instantiate<BooleanSettingInput>();
		input._value = value;
		input._labelText = label;
		return input;
	}
}

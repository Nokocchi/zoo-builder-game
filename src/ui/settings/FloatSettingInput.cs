using Godot;
using System;

public partial class FloatSettingInput : PanelContainer
{
	[Signal]
	public delegate void ValueChangedEventHandler(float newValue);
	
	private static readonly PackedScene FloatSettingInputScene = GD.Load<PackedScene>("res://src/ui/settings/float_setting_input.tscn");
	private HSlider _slider;
	private SpinBox _inputBox;
	private Label _label;
	
	private float _value;
	private string _labelText;

	public override void _Ready()
	{
		_slider = GetNode<HSlider>("%Slider");
		_inputBox = GetNode<SpinBox>("%InputBox");
		_label = GetNode<Label>("%Label");
		_slider.Value = _value;
		_inputBox.Value = _value;
		_label.Text = _labelText;
		_slider.ValueChanged += OnSliderValueChanged;
		_inputBox.ValueChanged += OnSTextValueChanged;
	}

	private void OnSliderValueChanged(double newValue)
	{
		_inputBox.SetValueNoSignal(newValue);
		EmitSignal(SignalName.ValueChanged, _value);
	}
	
	private void OnSTextValueChanged(double newValue)
	{
		_slider.SetValueNoSignal(newValue);
		EmitSignal(SignalName.ValueChanged, _value);
	}

	public static FloatSettingInput CreateWithValue(string label, float value)
	{
		FloatSettingInput input = FloatSettingInputScene.Instantiate<FloatSettingInput>();
		input._value = value;
		input._labelText = label;
		return input;
	}


}

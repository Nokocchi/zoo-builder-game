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
	private float _minValue;
	private float _maxValue;

	public override void _Ready()
	{
		_slider = GetNode<HSlider>("%Slider");
		_inputBox = GetNode<SpinBox>("%InputBox");
		_label = GetNode<Label>("%Label");
		_slider.Value = _value;
		_slider.MinValue = _minValue;
		_slider.MaxValue = _maxValue;
		_inputBox.Value = _value;
		_label.Text = _labelText;
		_slider.ValueChanged += OnSliderValueChanged;
		_inputBox.ValueChanged += OnSTextValueChanged;
	}

	private void OnSliderValueChanged(double newValue)
	{
		_inputBox.SetValueNoSignal(newValue);
		EmitSignal(SignalName.ValueChanged, newValue);
	}
	
	private void OnSTextValueChanged(double newValue)
	{
		_slider.SetValueNoSignal(newValue);
		EmitSignal(SignalName.ValueChanged, newValue);
	}

	public static FloatSettingInput CreateWithValue(string label, float value, float minValue, float maxValue)
	{
		FloatSettingInput input = FloatSettingInputScene.Instantiate<FloatSettingInput>();
		input._value = value;
		input._minValue = minValue;
		input._maxValue = maxValue;
		input._labelText = label;
		return input;
	}


}

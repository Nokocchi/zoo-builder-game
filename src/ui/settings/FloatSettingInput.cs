using Godot;
using System;
using ZooBuilder.ui.settings;

public partial class FloatSettingInput : PanelContainer, ISettingInput
{
	
	private static readonly PackedScene FloatSettingInputScene = GD.Load<PackedScene>("res://src/ui/settings/float_setting_input.tscn");
	private HSlider _slider;
	private SpinBox _inputBox;
	private Label _label;

	private float _minValue;
	private float _maxValue;

	public ISetting GetAsSetting() => new Setting<float>(SettingsKey, (float)_slider.Value);
	public string SettingsKey { get; private set; }
	private float _initialValue;

	public override void _Ready()
	{
		AddToGroup(GlobalDataSingleton.SETTINGS_INPUT_GROUP_NAME);
		_slider = GetNode<HSlider>("%Slider");
		_inputBox = GetNode<SpinBox>("%InputBox");
		_label = GetNode<Label>("%Label");
		_slider.Value = _initialValue;
		_inputBox.Value = _initialValue;
		_slider.MinValue = _minValue;
		_slider.MaxValue = _maxValue;
		_label.Text = SettingsKey;
		_slider.ValueChanged += OnSliderValueChanged;
		_inputBox.ValueChanged += OnSTextValueChanged;
	}

	private void OnSliderValueChanged(double newValue)
	{
		_inputBox.SetValueNoSignal(newValue);
	}
	
	private void OnSTextValueChanged(double newValue)
	{
		_slider.SetValueNoSignal(newValue);
	}

	public static FloatSettingInput CreateWithValue(string settingsKey, float value, float minValue, float maxValue)
	{
		FloatSettingInput input = FloatSettingInputScene.Instantiate<FloatSettingInput>();
		input._initialValue = value;
		input._minValue = minValue;
		input._maxValue = maxValue;
		input.SettingsKey = settingsKey;
		return input;
	}


}

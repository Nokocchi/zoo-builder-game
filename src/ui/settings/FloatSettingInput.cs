using Godot;
using System;
using ZooBuilder.ui.settings;

public partial class FloatSettingInput : PanelContainer, ISettingInput
{
	
	private static readonly PackedScene FloatSettingInputScene = GD.Load<PackedScene>("res://src/ui/settings/float_setting_input.tscn");
	private HSlider _slider;
	private SpinBox _inputBox;
	private Label _label;

	private FloatSetting _setting;

	public override void _Ready()
	{
		_slider = GetNode<HSlider>("%Slider");
		_inputBox = GetNode<SpinBox>("%InputBox");
		_label = GetNode<Label>("%Label");
		_slider.Value = _setting.GetValue();
		_inputBox.Value = _setting.GetValue();
		_slider.MinValue = _setting.MinValue;
		_slider.MaxValue = _setting.MaxValue;
		_label.Text = _setting.Key;
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
	
	public void SaveInputStateToGlobalSetting()
	{
		_setting.SaveNewValue((float) _slider.Value);
	}

	public static FloatSettingInput CreateWithValue( FloatSetting setting)
	{
		FloatSettingInput input = FloatSettingInputScene.Instantiate<FloatSettingInput>();
		input._setting = setting;
		return input;
	}
}

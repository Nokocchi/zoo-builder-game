using Godot;
using System;

public partial class Settings : CanvasLayer
{

	private HSlider _mouseSensitivitySlider;
	private SpinBox _mouseSensitivityText;
	private CheckBox _mouseUpDownFlipped;
	private CheckBox _hotbarScrollDirectionFlipped;
	private CheckBox _hideMinimap;
	private SettingsSingleton _settings;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_mouseSensitivitySlider = GetNode<HSlider>("%MouseSensitivitySlider");
		_mouseSensitivityText = GetNode<SpinBox>("%MouseSensitivityText");
		_mouseUpDownFlipped = GetNode<CheckBox>("%MouseUpDownFlipped");
		_hotbarScrollDirectionFlipped = GetNode<CheckBox>("%HotbarScrollDirectionFlipped");
		_hideMinimap = GetNode<CheckBox>("%HideMinimap");

		Visible = false;
		
		_settings = SettingsSingleton.Load();
		
		_settings.SettingsOpen = false;
		
		_mouseSensitivitySlider.Value = _settings.MouseSensitivity;
		_mouseSensitivityText.Value = _settings.MouseSensitivity;
		_mouseUpDownFlipped.SetPressed(_settings.MouseUpDownFlipped);
		_hotbarScrollDirectionFlipped.SetPressed(_settings.HotbarScrollDirectionFlipped);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnMouseSensitivitySliderUpdated(float mouseSensitivity)
	{
		_mouseSensitivityText.Value = mouseSensitivity;
		_settings.MouseSensitivity = mouseSensitivity;
		Save();
	}
	
	private void OnMouseSensitivityTextUpdated(float mouseSensitivity)
	{
		_mouseSensitivitySlider.Value = mouseSensitivity;
		_settings.MouseSensitivity = mouseSensitivity;
		Save();
	}

	private void OnMouseUpDownFlippedUpdated(bool mouseUpDownFlipped)
	{
		_settings.MouseUpDownFlipped = mouseUpDownFlipped;
		Save();
	}
	
	private void OnHotbarScrollDirectionFlipped(bool scrollbarDirectionFlipped)
	{
		_settings.HotbarScrollDirectionFlipped = scrollbarDirectionFlipped;
		Save();
	}
	
	private void OnHideMinimap(bool hideMinimap)
	{
		_settings.HideMinimap = hideMinimap;
		Save();
	}
	
	private void OnNorthFacingMinimap(bool northFacingMinimap)
	{
		_settings.NorthFacingMinimap = northFacingMinimap;
		Save();
	}

	private void Save()
	{
		ResourceSaver.Save(_settings, _settings.GetPath());
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("open_settings"))
		{
			_settings.SettingsOpen = !_settings.SettingsOpen;
			Visible = !Visible;
			Input.MouseMode = Visible ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
		}
	}
}

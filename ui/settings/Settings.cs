using Godot;
using System;

public partial class Settings : CanvasLayer
{

	private HSlider _mouseSensitivitySlider;
	private SpinBox _mouseSensitivityText;
	private HSlider _audioLevelSlider;
	private SpinBox _audioLevelText;
	private CheckBox _mouseUpDownFlipped;
	private CheckBox _hotbarScrollDirectionFlipped;
	private CheckBox _hideMinimap;
	private SettingsResource _settings;
	private int _audioBusIndexMaster;
	private int _audioBusIndexBgMusic;
	private int _audioBusIndexSfx;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_mouseSensitivitySlider = GetNode<HSlider>("%MouseSensitivitySlider");
		_mouseSensitivityText = GetNode<SpinBox>("%MouseSensitivityText");
		_audioLevelSlider = GetNode<HSlider>("%AudioLevelSlider");
		_audioLevelText = GetNode<SpinBox>("%AudioLevelText");
		_mouseUpDownFlipped = GetNode<CheckBox>("%MouseUpDownFlipped");
		_hotbarScrollDirectionFlipped = GetNode<CheckBox>("%HotbarScrollDirectionFlipped");
		_hideMinimap = GetNode<CheckBox>("%HideMinimap");

		Visible = false;
		
		_settings = SettingsResource.Load();
		
		_settings.SettingsOpen = false;
		
		_mouseSensitivitySlider.Value = _settings.MouseSensitivity;
		_mouseSensitivityText.Value = _settings.MouseSensitivity;
		_audioLevelSlider.Value = _settings.BackgroundAudioVolume;
		_audioLevelText.Value = _settings.BackgroundAudioVolume;
		_mouseUpDownFlipped.SetPressed(_settings.MouseUpDownFlipped);
		_hotbarScrollDirectionFlipped.SetPressed(_settings.HotbarScrollDirectionFlipped);

		_audioBusIndexMaster = AudioServer.GetBusIndex("Master");
		_audioBusIndexBgMusic = AudioServer.GetBusIndex("BgMusic");
		_audioBusIndexSfx = AudioServer.GetBusIndex("SFX");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnMouseSensitivitySliderUpdated(float mouseSensitivity)
	{
		_mouseSensitivityText.Value = mouseSensitivity;
		_settings.MouseSensitivity = mouseSensitivity;
		_settings.Save();
	}
	
	private void OnMouseSensitivityTextUpdated(float mouseSensitivity)
	{
		_mouseSensitivitySlider.Value = mouseSensitivity;
		_settings.MouseSensitivity = mouseSensitivity;
		_settings.Save();
	}
	
	private void OnBackgroundAudioVolumeSliderUpdated(float audioVolume)
	{
		//_audioLevelText.Value = audioVolumeAdjusted;
		_settings.BackgroundAudioVolume = audioVolume;
		AudioServer.SetBusVolumeLinear(_audioBusIndexMaster,  audioVolume / 100);
		_settings.Save();
	}
	
	private void OnBackgroundAudioVolumeTextUpdated(float audioVolume)
	{
		//_audioLevelSlider.Value = audioVolumeAdjusted;
		_settings.BackgroundAudioVolume = audioVolume;
		AudioServer.SetBusVolumeLinear(_audioBusIndexMaster,  audioVolume / 100);
		_settings.Save();
	}

	private void OnMouseUpDownFlippedUpdated(bool mouseUpDownFlipped)
	{
		_settings.MouseUpDownFlipped = mouseUpDownFlipped;
		_settings.Save();
	}
	
	private void OnHotbarScrollDirectionFlipped(bool scrollbarDirectionFlipped)
	{
		_settings.HotbarScrollDirectionFlipped = scrollbarDirectionFlipped;
		_settings.Save();
	}
	
	private void OnHideMinimap(bool hideMinimap)
	{
		_settings.HideMinimap = hideMinimap;
		_settings.Save();
	}
	
	private void OnNorthFacingMinimap(bool northFacingMinimap)
	{
		_settings.NorthFacingMinimap = northFacingMinimap;
		_settings.Save();
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

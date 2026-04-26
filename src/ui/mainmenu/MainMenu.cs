using Godot;
using System;
using ZooBuilder.globals;

public partial class MainMenu : CanvasLayer
{

	[Export] public PackedScene GameScene;
	private PanelContainer _settingsContainer;
	private static readonly PackedScene SettingsScene = GD.Load<PackedScene>("res://src/ui/ingamemenu/settings/settings.tscn");
	
	public override void _Ready()
	{
		GlobalDataSingleton.LoadSettingsFromDisk();
		TranslationServer.SetLocale(GlobalDataSingleton.SelectedLocale);
		_settingsContainer = GetNode<PanelContainer>("%SettingsContainer");
	}

	
	public override void _Process(double delta)
	{
	}

	private void OnPlayPressed()
	{
		GetTree().ChangeSceneToPacked(GameScene);
	}
	
	private void OnSetingsPressed()
	{
		Settings settingsScene = SettingsScene.Instantiate<Settings>();
		_settingsContainer.AddChild(settingsScene);
		settingsScene.Initialize();
	}
	
	private void OnQuitPressed()
	{
		GetTree().Quit();
	}
}

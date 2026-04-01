using Godot;
using System;
using ZooBuilder.globals;

public partial class MainMenu : CanvasLayer
{

	[Export] public PackedScene GameScene;
	
	public override void _Ready()
	{
		TranslationServer.SetLocale(SettingsResource.Load().SelectedLocale);
		InputManager.LoadActions();
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
		
	}
	
	private void OnQuitPressed()
	{
		GetTree().Quit();
	}
}

using Godot;
using System;

public partial class MainMenu : CanvasLayer
{

	[Export] public PackedScene GameScene;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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

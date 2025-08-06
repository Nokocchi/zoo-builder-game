using Godot;
using System;

public partial class GameSaveLabel : CanvasLayer
{
	private Label _label;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_label = GetNode<Label>("%GameSaveLabel");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public async void ShowText(Action saveAction)
	{
		_label.Text = "Saving..";
		saveAction.Invoke();
		await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);
		_label.Text = "";
	}
}

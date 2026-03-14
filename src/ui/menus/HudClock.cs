using Godot;
using System;

public partial class HudClock : CanvasLayer
{

	private Label _label;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_label = GetNode<Label>("%ClockLabel");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnTimeUpdated(int gameTimeSeconds)
	{
		int timeSecondsPart = gameTimeSeconds % 60;
		double timeMinutesPart = Math.Floor(gameTimeSeconds / 60d);
		_label.Text = $"{timeMinutesPart}:{timeSecondsPart}";
	}
}

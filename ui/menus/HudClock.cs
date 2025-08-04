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

	private void OnTimeUpdated(TimeResource timeResource)
	{
		int totalTimeSeconds = timeResource.GameTime;
		int timeSecondsPart = totalTimeSeconds % 60;
		double timeMinutesPart = Math.Floor(totalTimeSeconds / 60d);
		_label.Text = $"{timeMinutesPart}:{timeSecondsPart}";
	}
}

using Godot;

namespace SquashtheCreeps3D;

public partial class Item(ItemName name, string description) : Node3D
{
	public ItemName ItemName { get; } = name;
	public string Description { get; } = description;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
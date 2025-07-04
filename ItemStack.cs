using Godot;

namespace SquashtheCreeps3D;

public partial class ItemStack(Item item, int amount) : Node3D
{
	// Called when the node enters the scene tree for the first time.

	public readonly Item Item = item;
	public int Amount = amount;

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void IncreaseStackSize(int amount)
	{
		Amount += amount;
	}
}
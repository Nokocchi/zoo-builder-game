using Godot;

[GlobalClass]
public partial class ItemStackResource(ItemDataResource itemData, int amount) : Resource
{
	// Called when the node enters the scene tree for the first time.

	[field: Export] public ItemDataResource ItemData { get; } = itemData;
	public int Amount = amount;

	public void IncreaseStackSize(int amount)
	{
		Amount += amount;
	}
}
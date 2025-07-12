using Godot;

public partial class ItemStackResource(ItemDataResource itemData, int amount) : Resource
{
	// Called when the node enters the scene tree for the first time.

	public readonly ItemDataResource ItemData = itemData;
	public int Amount = amount;

	public void IncreaseStackSize(int amount)
	{
		Amount += amount;
	}
}
using Godot;

[GlobalClass]
public partial class ItemStackResource : Resource
{
    
    // Empty constructor so this item can be instantiated from the editor
    public ItemStackResource()
    {
    }

    public ItemStackResource(ItemDataResource itemData, int amount)
    {
        ItemData = itemData;
        Amount = amount;
    }

    [Export] public ItemDataResource ItemData { get; set; }
    [Export] public int Amount { get; set; }

    public void IncreaseStackSize(int amount)
    {
        Amount += amount;
    }

    public int SplitInHalf()
    {
        int removedAmount = Amount / 2;
        Amount -= removedAmount;
        return removedAmount;
    }
}
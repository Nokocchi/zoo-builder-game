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
    
    [Export] public ItemDataResource ItemData { get; private set; }
    [Export] public int Amount { get; private set; }

    public void SetItemData(ItemDataResource item)
    {
        ItemData = item;
    }
    
    public void Update(ItemStackResource item)
    {
        ItemData = item.ItemData;
        Amount = item.Amount;
    }

    public bool HasItem()
    {
        return ItemData != null;
    }

    public void IncreaseStackSize(int amount)
    {
        Amount += amount;
    }
    
    public int DecrementStackSize()
    {
        Amount--;
        return Amount;
    }

    public int SplitInHalf()
    {
        int removedAmount = Amount / 2;
        Amount -= removedAmount;
        return removedAmount;
    }
}
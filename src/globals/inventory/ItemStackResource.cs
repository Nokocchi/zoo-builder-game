using Godot;

[GlobalClass]
public partial class ItemStackResource : Resource
{
    
    [Signal]
    public delegate void StackSizeChangedEventHandler();
    
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
        EmitSignal(SignalName.StackSizeChanged);
    }
    
    public int DecrementStackSize()
    {
        Amount--;
        EmitSignal(SignalName.StackSizeChanged);
        return Amount;
    }

    public int SplitInHalf()
    {
        int removedAmount = Amount / 2;
        Amount -= removedAmount;
        EmitSignal(SignalName.StackSizeChanged);
        return removedAmount;
    }
}
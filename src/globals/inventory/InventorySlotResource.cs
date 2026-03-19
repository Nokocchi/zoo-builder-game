using Godot;

[GlobalClass]
public partial class InventorySlotResource : Resource
{
    // Maybe provide the InventorySingleton an index + Callback method collection so only InventorySingleton can call these methods?
    [Signal]
    public delegate void StackSizeChangedEventHandler();
    

    public InventorySlotResource(int inventoryIndex, ItemStackResource itemStack)
    {
        InventoryIndex = inventoryIndex;
        _itemStack = itemStack;
    }

    public int InventoryIndex { get; private set; }
    private ItemStackResource _itemStack;

    public void SetItemStack(ItemStackResource itemStack)
    {
        _itemStack = itemStack;
    }

    public bool HasItem()
    {
        return _itemStack != null;
    }
    
    public bool IsEmpty()
    {
        return _itemStack == null;
    }

    public ItemStackResource GetItem()
    {
        return _itemStack;
    }

    public void IncreaseStackSize(int amount)
    {
        _itemStack.IncreaseStackSize(amount);
        EmitSignal(SignalName.StackSizeChanged);
    }
    
    public int DecrementStackSize()
    {
        int newAmount = _itemStack.DecrementStackSize();
        EmitSignal(SignalName.StackSizeChanged);
        return newAmount;
    }

    public int SplitInHalf()
    {
        int removed = _itemStack.SplitInHalf();
        EmitSignal(SignalName.StackSizeChanged);
        return removed;
    }

    public void ClearStack()
    {
        _itemStack = null;
        EmitSignal(SignalName.StackSizeChanged);
    }
}
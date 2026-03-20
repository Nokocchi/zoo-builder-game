using Godot;
using ZooBuilder.entities.player;

[GlobalClass]
public partial class InventorySlotResource : Resource
{
    // Maybe provide the InventorySingleton an index + Callback method collection so only InventorySingleton can call these methods?
    [Signal]
    public delegate void SlotContentChangedEventHandler();
    

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
        TriggerChangeHandler();
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
        TriggerChangeHandler();
    }
    
    public int DecrementStackSize()
    {
        int newAmount = _itemStack.DecrementStackSize();
        TriggerChangeHandler();
        return newAmount;
    }

    public int SplitInHalf()
    {
        int removed = _itemStack.SplitInHalf();
        TriggerChangeHandler();
        return removed;
    }

    public void ClearStack()
    {
        _itemStack = null;
        TriggerChangeHandler();
    }

    private void TriggerChangeHandler()
    {
        EmitSignal(SignalName.SlotContentChanged);
    }
}
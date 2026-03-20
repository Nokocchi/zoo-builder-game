using Godot;

[GlobalClass]
public partial class HeldItem : Resource
{

    public HeldItem(ItemStackResource itemStackResource, int originatesFromInventoryIndex)
    {
        // Create **new** InventorySlotResource so that changes in our held item don't take effect in the inventory as well
        SlotResource = new InventorySlotResource(originatesFromInventoryIndex, itemStackResource);
    }

    public InventorySlotResource SlotResource { get; private set; }

    public int DecrementStackSize()
    {
        return SlotResource.DecrementStackSize();
    }

    public ItemStackResource GetItemStack()
    {
        return SlotResource.GetItem();
    }

    public int GetOriginatesFromIndex()
    {
        return SlotResource.InventoryIndex;
    }
}
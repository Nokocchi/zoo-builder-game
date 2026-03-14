using Godot;

[GlobalClass]
public partial class HeldItem : Resource
{
    public HeldItem()
    {
    }

    public HeldItem(ItemStackResource itemStackResource, int originatesFromInventoryIndex)
    {
        ItemStackResource = itemStackResource;
        OriginatesFromInventoryIndex = originatesFromInventoryIndex;
    }

    public ItemStackResource ItemStackResource { get; }
    public int OriginatesFromInventoryIndex { get; }
}
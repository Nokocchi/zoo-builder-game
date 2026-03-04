using Godot;
using System;
using ZooBuilder.globals;

// Is control so that it renders on top of other menus, like Inventory
public partial class MouseWithItemMarker : Control
{
    private InventoryItemStack _itemStackInstance;
    private PackedScene _inventoryItemStackScene;

    public override void _Ready()
    {
        _itemStackInstance = GetNode<InventoryItemStack>("InventoryItemStack");
        Visible = false;
        InventorySingleton.Instance.InventoryItemStackHeld += OnItemHeld;
    }

    private void OnItemHeld(HeldItem heldItem)
    {
        if (heldItem == null)
        {
            _itemStackInstance.ItemStackResource = null;
            Visible = false;
        }
        else
        {
            _itemStackInstance.InventoryIndex = heldItem.OriginatesFromInventoryIndex;
            _itemStackInstance.ItemStackResource = heldItem.ItemStackResource;
            Visible = true;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion && _itemStackInstance != null)
        {
            // Itemstack's bottom left corner follows mouse. When clicking, you technically click the InventoryItemStack, but this is marked as mouse = ignore. 
            Position = new Vector2(eventMouseMotion.Position.X, (eventMouseMotion.Position.Y - _itemStackInstance.Size.Y));
            // get_viewport().get_mouse_position() ?
        }
        
        if (!@event.IsActionPressed("toss_single_item")) return;
        if (_itemStackInstance.ItemStackResource == null) return;
        if (_itemStackInstance.ItemStackResource.Amount <= 0) return;
        
        // Spawn item first, in case we remove the itemData in the decrement call below
        OverworldItem.SpawnItemAndLaunchFromPlayer(new ItemStackResource(_itemStackInstance.ItemStackResource.ItemData, 1));
        // This updates the **item data resource** and will take effect in the InventorySingleton as well 
        _itemStackInstance.DecrementRerenderAndRemoveIfZero();
        
        // Consuming the event to avoid the HotBar thinking it's time to drop the item that is in focus, just because this class has dropped the last of its stack
        GetWindow().SetInputAsHandled();
    }
}
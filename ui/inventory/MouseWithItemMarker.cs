using Godot;
using System;
using ZooBuilder.globals;

// Is control so that it renders on top of other menus, like Inventory
public partial class MouseWithItemMarker : Control
{
    // TODO: This InventoryItemStack is purely for show. The real item resource is in the InventorySingleton. Make this more clear? Or maybe it's okay if it's copied by refeence?
    private InventoryItemStack _itemStackInstance;
    private int _heldIndex = -1;
    
    public override void _Ready()
    {
        GlobalObjectsContainer.Instance.MouseWithMarker = this;
        _itemStackInstance = GetNode<InventoryItemStack>("InventoryItemStack");
        Visible = false;
        InventorySingleton.Instance.InventoryItemStackHeld += OnItemHeld;
    }

    private void OnItemHeld(int heldIndex)
    {
        if (heldIndex < 0)
        {
            ClearItemStack();
        }
        else
        {
            _heldIndex = heldIndex;
            ShowItemStack();
        }
    }

    private void ShowItemStack()
    {
        if (_heldIndex < 0) return;
        ItemStackResource itemStackResource = InventorySingleton.Instance.Inventory[_heldIndex];
        Visible = true;
        _itemStackInstance.ItemStackResource = itemStackResource;
    }
    
    private void ClearItemStack()
    {
        Visible = false;
        _itemStackInstance.ItemStackResource = null;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            // Itemstack's bottom left corner follows mouse. When clicking, you technically click the InventoryItemStack, but this is marked as mouse = ignore. 
            Position = new Vector2(eventMouseMotion.Position.X, (eventMouseMotion.Position.Y - _itemStackInstance.Size.Y));
            // get_viewport().get_mouse_position() ?
        }

        if (@event.IsActionPressed("toss_single_item") && InventorySingleton.Instance.HoldsItem)
        {
            
            ItemStackResource itemStackResource = InventorySingleton.Instance.Inventory[_heldIndex];
            if (itemStackResource != null && itemStackResource.BeingHeld && itemStackResource.Amount > 0)
            {
                _itemStackInstance.DecrementRerenderAndRemoveIfZero();
                OverworldItem.SpawnItemAndLaunchFromPlayer(
                    new ItemStackResource(_itemStackInstance.ItemStackResource.ItemData, 1));
            }
        }
    }
}
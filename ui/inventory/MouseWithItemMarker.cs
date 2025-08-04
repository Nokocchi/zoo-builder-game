using Godot;
using System;
using ZooBuilder.globals;

// Is control so that it renders on top of other menus, like Inventory
public partial class MouseWithItemMarker : Control
{
    // TODO: This InventoryItemStack is purely for show. The real item resource is in the InventorySingleton. Make this more clear? Or maybe it's okay if it's copied by refeence?
    private InventoryItemStack _itemStackInstance;

    public override void _Ready()
    {
        _itemStackInstance = GetNode<InventoryItemStack>("InventoryItemStack");
        Visible = false;
        InventorySingleton.Instance.InventoryItemStackHeld += OnItemHeld;
        InventorySingleton.Instance.InventoryItemStackNoLongerHeld += OnItemNoLongerHeld;
    }

    private void OnItemHeld(int heldIndex)
    {
        _itemStackInstance.InventoryIndex = heldIndex;
        _itemStackInstance.ItemStackResource = InventorySingleton.Instance.Inventory[heldIndex];
        Visible = true;
    }

    private void OnItemNoLongerHeld(int heldIndex)
    {
        Visible = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            // Itemstack's bottom left corner follows mouse. When clicking, you technically click the InventoryItemStack, but this is marked as mouse = ignore. 
            Position = new Vector2(eventMouseMotion.Position.X,
                (eventMouseMotion.Position.Y - _itemStackInstance.Size.Y));
            // get_viewport().get_mouse_position() ?
        }

        if (!@event.IsActionPressed("toss_single_item") || !InventorySingleton.Instance.HoldsItem) return;

        ItemStackResource itemStackResource = InventorySingleton.Instance.Inventory[_itemStackInstance.InventoryIndex];

        if (itemStackResource is not { BeingHeld: true } || itemStackResource.Amount <= 0) return;
        GD.Print("Q from HeldItem");
        OverworldItem.SpawnItemAndLaunchFromPlayer(
            new ItemStackResource(_itemStackInstance.ItemStackResource.ItemData, 1));
        _itemStackInstance.DecrementRerenderAndRemoveIfZero();
    }
}
using Godot;
using System;
using ZooBuilder.globals;

// Is control so that it renders on top of other menus, like Inventory
public partial class MouseWithItemMarker : Control
{
    private InventoryItemStack _itemStackInstance;

    public InventoryItemStack ItemStackInstance => _itemStackInstance;
    
    public override void _Ready()
    {
        GlobalObjectsContainer.Instance.MouseWithMarker = this;
        _itemStackInstance = GetNode<InventoryItemStack>("InventoryItemStack");
        Visible = false;
        InventorySingleton.Instance.InventoryItemStackHeld += OnItemHeld;
    }

    private void OnItemHeld(int indexHeld)
    {
        if (indexHeld == -1)
        {
            GD.Print("Clear");
            ClearItemStack();
        }
        else
        {
            ItemStackResource itemStackResource = InventorySingleton.Instance.Inventory[indexHeld];
            HoldItemStack(itemStackResource, indexHeld);
        }
    }

    public void HoldItemStack(ItemStackResource holdStack, int indexHeld)
    {
        if (holdStack == null)
        {
            return;
        }

        Visible = true;
        _itemStackInstance.ItemStackResource = holdStack;
        _itemStackInstance.InventoryIndex = indexHeld;
    }
    
    public void ClearItemStack()
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
            
            // TODO:
            // When picking up an item, should the inventory list in InventorySingleton be changed? Or do we leave it until an action has been taken?
            // How does MouseWithItemMarker throw the item into the overworld? With a signal?
            // When an item is down to 0 amount, who is in charge of removing it from the inventorySingleton and the visual inventories? 
            _itemStackInstance.DecrementAndRerender();
            OverworldItem.SpawnItemAndLaunchFromPlayer(
                new ItemStackResource(_itemStackInstance.ItemStackResource.ItemData, 1));
        }
    }
}
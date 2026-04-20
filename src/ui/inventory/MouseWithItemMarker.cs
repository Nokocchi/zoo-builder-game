using Godot;
using System;
using ZooBuilder.globals;
using ZooBuilder.ui.inventory;

// Is control so that it renders on top of other menus, like Inventory
public partial class MouseWithItemMarker : Control
{
    private UiInventorySlot _uiSlotInstance;
    private IInventory _inventory;

    public override void _Ready()
    {
        _uiSlotInstance = GetNode<UiInventorySlot>("InventoryItemStack");
        Visible = false;
        _inventory = InventorySingleton.Instance;
        EventBus.Subscribe<InventoryItemStackHeldEvent>(OnItemHeld);
    }

    private void OnItemHeld(InventoryItemStackHeldEvent e)
    {
        HeldItem heldItem = e.HeldItem;
        if (heldItem == null)
        {
            Visible = false;
        }
        else
        {
            _uiSlotInstance.SetInventorySlotResource(e.HeldItem.SlotResource);
            Visible = true;
        }
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion && _uiSlotInstance != null)
        {
            // Itemstack's bottom left corner follows mouse. When clicking, you technically click the InventoryItemStack, but this is marked as mouse = ignore. 
            Position = new Vector2(eventMouseMotion.Position.X, (eventMouseMotion.Position.Y - _uiSlotInstance.Size.Y));
            // get_viewport().get_mouse_position() ?
        }

        if (!@event.IsActionPressed(GlobalDataSingleton.ACTION_TOSS_SINGLE_ITEM)) return;
        if (!Visible || _uiSlotInstance.InventorySlotResource.IsEmpty() || _uiSlotInstance.InventorySlotResource.GetItem().Amount <= 0) return;
        
        _inventory.TossOneOfHeldItem();

        // Consuming the event to avoid the HotBar thinking it's time to drop the item that is in focus, just because this class has dropped the last of its stack
        GetWindow().SetInputAsHandled();
    }
}
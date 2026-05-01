using Godot;
using ZooBuilder.globals;

namespace ZooBuilder.ui.inventory;

public abstract partial class AbstractInventoryUi : Control
{
    private PackedScene _inventoryItemStackScene;
    protected Container UiSlotContainer;
    protected int FirstSlot;
    protected int LastSlot;
    
    public override void _Ready()
    {
        _inventoryItemStackScene = GD.Load<PackedScene>("res://src/ui/ingamemenu/inventory/ui_inventory_slot.tscn");
        EventBus.Subscribe<GameFinishedLoadingEvent>(OnSaveDataLoaded);
    }

    public override void _ExitTree()
    {
        EventBus.Unsubscribe<GameFinishedLoadingEvent>(OnSaveDataLoaded);
    }
    
    protected virtual void OnSaveDataLoaded(GameFinishedLoadingEvent e)
    {
        IInventory inventorySingleton = InventorySingleton.Instance;
        for (int i = FirstSlot; i <= LastSlot; i++)
        {
            // Could also have been a static Initialize() method, and instead of SlotClicked signals, just call inventorySingleton.ItemClicked directly from within UiInventorySlot.
            UiInventorySlot uiInventorySlot = _inventoryItemStackScene.Instantiate<UiInventorySlot>();
            UiSlotContainer.AddChild(uiInventorySlot); // Add to tree to run _Ready() before setting any resources
            uiInventorySlot.SlotClicked += (clickedSlotIndex) => inventorySingleton.ItemClicked(clickedSlotIndex);
            uiInventorySlot.SlotRightClicked += (clickedSlotIndex) => inventorySingleton.ItemRightClicked(clickedSlotIndex);
            InventorySlotResource inventorySlotResource = inventorySingleton.GetInventory()[i];
            uiInventorySlot.SetInventorySlotResource(inventorySlotResource);
        }
    }

}
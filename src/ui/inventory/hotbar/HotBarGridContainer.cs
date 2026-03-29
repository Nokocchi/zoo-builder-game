using Godot;
using ZooBuilder.entities.player;
using ZooBuilder.globals;
using ZooBuilder.ui.inventory;

public partial class HotBarGridContainer : InventoryHandler
{
    private SettingsResource _settings;
    public int SelectedHotbarIndex { get; private set; }

    public override void _Ready()
    {
        firstSlot = 0;
        lastSlot = InventorySingleton.HotBarSize - 1;
        base._Ready();
        _settings = SettingsResource.Load();
        
        // Select first slot on startup
        GetChild<UiInventorySlot>(SelectedHotbarIndex).HighlightSlot();
        // Set first slot as the selected slot in the itemInHand mesh
        EventBus.Publish(new SelectedHotbarSlotChangedItemEvent(SelectedHotbarIndex)); 
    }


    // Change selected hotbar slot with mouse scrolling
    public override void _Input(InputEvent @event)
    {
        if (UIManager.IsMenuOpen()) return;
        
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.IsPressed())
        {
            bool wheelDown = eventMouseButton.ButtonIndex == MouseButton.WheelDown;
            bool wheelUp = eventMouseButton.ButtonIndex == MouseButton.WheelUp;

            // Player is not scrolling - end early
            if (!wheelDown && !wheelUp)
            {
                return;
            }


            int previousIndex = SelectedHotbarIndex;
            if (wheelDown ^ _settings.HotbarScrollDirectionFlipped)
            {
                SelectedHotbarIndex = (SelectedHotbarIndex + 1) % InventorySingleton.HotBarSize;
            }

            if (wheelUp ^ _settings.HotbarScrollDirectionFlipped)
            {
                SelectedHotbarIndex = ((SelectedHotbarIndex - 1) + InventorySingleton.HotBarSize) % InventorySingleton.HotBarSize;
            }

            GetChild<UiInventorySlot>(previousIndex).RemoveHighlight();
            GetChild<UiInventorySlot>(SelectedHotbarIndex).HighlightSlot();
            EventBus.Publish(new SelectedHotbarSlotChangedItemEvent(SelectedHotbarIndex));
        }

        if (!@event.IsActionPressed("toss_single_item")) return;
        UiInventorySlot currentSelectedStack = GetChild<UiInventorySlot>(SelectedHotbarIndex);
        if (_inventorySingleton.GetHeldItem() != null || currentSelectedStack?.InventorySlotResource?.GetItem() == null || currentSelectedStack.InventorySlotResource.GetItem() is not { Amount: >= 0 }) return;
        _inventorySingleton.TossOneOfItem(currentSelectedStack.InventorySlotResource.InventoryIndex);
    }
}
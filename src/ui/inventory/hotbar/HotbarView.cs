using Godot;
using ZooBuilder.entities.player;
using ZooBuilder.globals;
using ZooBuilder.ui.inventory;

public partial class HotbarView : InventoryHandler
{
    public int SelectedHotbarIndex { get; private set; }

    public override void _Ready()
    {
        FirstSlot = 0;
        LastSlot = global::InventorySingleton.HotBarSize - 1;
        base._Ready();
        
        // Select first slot on startup
        UiSlotContainer.GetChild<UiInventorySlot>(SelectedHotbarIndex).HighlightSlot();
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
            if (wheelDown ^ GlobalData.HotbarScrollDirectionFlipped)
            {
                SelectedHotbarIndex = (SelectedHotbarIndex + 1) % global::InventorySingleton.HotBarSize;
            }

            if (wheelUp ^ GlobalData.HotbarScrollDirectionFlipped)
            {
                SelectedHotbarIndex = ((SelectedHotbarIndex - 1) + global::InventorySingleton.HotBarSize) % global::InventorySingleton.HotBarSize;
            }

            UiSlotContainer.GetChild<UiInventorySlot>(previousIndex).RemoveHighlight();
            UiSlotContainer.GetChild<UiInventorySlot>(SelectedHotbarIndex).HighlightSlot();
            EventBus.Publish(new SelectedHotbarSlotChangedItemEvent(SelectedHotbarIndex));
        }

        if (!@event.IsActionPressed(InputManager.ACTION_TOSS_SINGLE_ITEM)) return;
        UiInventorySlot currentSelectedStack = UiSlotContainer.GetChild<UiInventorySlot>(SelectedHotbarIndex);
        if (InventorySingleton.GetHeldItem() != null || currentSelectedStack?.InventorySlotResource?.GetItem() == null || currentSelectedStack.InventorySlotResource.GetItem() is not { Amount: >= 0 }) return;
        InventorySingleton.TossOneOfItem(currentSelectedStack.InventorySlotResource.InventoryIndex);
    }
}
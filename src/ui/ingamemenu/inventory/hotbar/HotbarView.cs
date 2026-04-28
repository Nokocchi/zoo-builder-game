using Godot;
using ZooBuilder.entities.player;
using ZooBuilder.globals;
using ZooBuilder.ui.inventory;

public partial class HotbarView : AbstractInventoryUi
{
    public int SelectedHotbarIndex { get; private set; }

    public override void _Ready()
    {
        FirstSlot = 0;
        LastSlot = InventorySingleton.HotBarSize - 1;
        UiSlotContainer = GetNode<Container>("%UISlotContainer");
        base._Ready();
    }

    protected override void OnSaveDataLoaded(GameFinishedLoadingEvent e)
    {
        GD.Print("INVENTORY STUFF hotbar");
        base.OnSaveDataLoaded(e);
        
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
            if (wheelDown ^ GlobalDataSingleton.HotbarScrollDirectionFlipped)
            {
                SelectedHotbarIndex = (SelectedHotbarIndex + 1) % InventorySingleton.HotBarSize;
            }

            if (wheelUp ^ GlobalDataSingleton.HotbarScrollDirectionFlipped)
            {
                SelectedHotbarIndex = ((SelectedHotbarIndex - 1) + InventorySingleton.HotBarSize) % InventorySingleton.HotBarSize;
            }

            UiSlotContainer.GetChild<UiInventorySlot>(previousIndex).RemoveHighlight();
            UiSlotContainer.GetChild<UiInventorySlot>(SelectedHotbarIndex).HighlightSlot();
            EventBus.Publish(new SelectedHotbarSlotChangedItemEvent(SelectedHotbarIndex));
        }

        if (!@event.IsActionPressed(GlobalDataSingleton.ACTION_TOSS_SINGLE_ITEM)) return;
        
        IInventory inventorySingleton = InventorySingleton.Instance;
        UiInventorySlot currentSelectedStack = UiSlotContainer.GetChild<UiInventorySlot>(SelectedHotbarIndex);
        if (inventorySingleton.GetHeldItem() != null || currentSelectedStack?.InventorySlotResource?.GetItem() == null || currentSelectedStack.InventorySlotResource.GetItem() is not { Amount: >= 0 }) return;
        inventorySingleton.TossOneOfItem(currentSelectedStack.InventorySlotResource.InventoryIndex);
    }
}
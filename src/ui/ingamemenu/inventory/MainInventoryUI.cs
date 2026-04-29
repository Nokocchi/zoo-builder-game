using Godot;
using ZooBuilder.globals;

namespace ZooBuilder.ui.inventory;

public partial class MainInventoryUI : AbstractInventoryUi
{
    public override void _Ready()
    {
        FirstSlot = InventorySingleton.HotBarSize;
        UiSlotContainer = GetNode<Container>("%UISlotContainer");
        base._Ready();
    }

    protected override void OnSaveDataLoaded(GameFinishedLoadingEvent e)
    { 
        LastSlot = InventorySingleton.Instance.GetInventory().Capacity - 1;
        base.OnSaveDataLoaded(e);
    }
}
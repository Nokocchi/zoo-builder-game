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
        GD.Print("INVENTORY STUFF main: ", FirstSlot);
        LastSlot = InventorySingleton.Instance.GetInventory().Capacity - 1;
        GD.Print("INVENTORY STUFF main: ", LastSlot);
        GD.Print("NVENTORY STUFF Setting last slot: ", LastSlot);
        GD.Print("NVENTORY STUFF Instance from main UI: ", InventorySingleton.Instance);
        base.OnSaveDataLoaded(e);
    }
}
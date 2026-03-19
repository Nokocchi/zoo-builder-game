namespace ZooBuilder.ui.inventory;

public partial class MainInventoryGridContainer : InventoryHandler
{
    public override void _Ready()
    {
        firstSlot = InventorySingleton.HotBarSize;
        lastSlot = InventorySingleton.Instance.GetInventory().Count - 1;
        base._Ready();
    }
}
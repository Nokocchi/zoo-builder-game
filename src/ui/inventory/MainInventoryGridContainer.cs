namespace ZooBuilder.ui.inventory;

public partial class MainInventoryGridContainer : InventoryHandler
{
    public override void _Ready()
    {
        slotsCount = InventorySingleton.Instance.GetInventory().Count - InventorySingleton.HotBarSize;
        base._Ready();
    }
}
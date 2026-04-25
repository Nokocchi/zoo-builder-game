namespace ZooBuilder.ui.inventory;

public partial class MainInventoryUI : AbstractInventoryUi
{
    public override void _Ready()
    {
        FirstSlot = global::InventorySingleton.HotBarSize;
        LastSlot = global::InventorySingleton.Instance.GetInventory().Count - 1;
        base._Ready();
    }
}
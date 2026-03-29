using System.Collections.Generic;

namespace ZooBuilder.globals;

public interface IInventory
{
    List<InventorySlotResource> GetInventory();
    
    HeldItem GetHeldItem();
    void TossEntireHeldItemStack();
    void TossOneOfHeldItem();
    
    bool TryAddItem(ItemStackResource itemStack);
    
    void ItemClicked(int itemIndex);
    void ItemRightClicked(int itemIndex);

    void TossOneOfItem(int inventoryIndex);
}
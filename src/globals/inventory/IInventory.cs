using System.Collections.Generic;

namespace ZooBuilder.globals;

public interface IInventory
{
    bool TryAddItem(ItemStackResource itemStack);
    void RemoveStackFromInventory(int index);
    void ItemClicked(int itemIndex);
    void DropHeldItem();
    List<ItemStackResource> GetInventory();
    bool IsMenuOpen(); // TODO: Probably not the right place to store this variable
    void SetMenuOpen(bool open); // TODO: Probably not the right place to store this variable
    HeldItem GetHeldItem();
}
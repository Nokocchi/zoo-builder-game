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
 

    // TODO inv: Use EventBus to define signals, and convert all usages of the inventory singleton to just use the interface
    //  Could I somehow show the events in this interface?
}
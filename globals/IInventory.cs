namespace ZooBuilder.globals;

public interface IInventory
{
    bool TryAddItem(ItemStackResource itemStack);
    void RemoveStackFromInventory(int index);
    void ItemClicked(int itemIndex);
}
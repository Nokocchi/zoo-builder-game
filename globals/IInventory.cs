namespace ZooBuilder.globals;

public interface IInventory
{
    bool TryAddItem(ItemStackResource itemStack);
    void RemoveStackFromInventory(int index);
    void ItemClicked(int itemIndex);
    void DropHeldItem();

    // TODO: It's not possible to define signals here. Consider converting to pure C# events instead of signals, but then it's necessary to manually unsubscribe on ExitTree
    // https://github.com/godotengine/godot/issues/70414
}
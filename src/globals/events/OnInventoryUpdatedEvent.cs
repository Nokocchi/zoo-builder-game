namespace ZooBuilder.ui.inventory;

// Update the inventory when new stacks are added, stacks are removed or stacks are swapped. Not for increment/decrement/split, as this is handled in the InventoryItemStack itself.
public record OnInventoryUpdatedEvent();
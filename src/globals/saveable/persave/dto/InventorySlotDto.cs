using System.Collections.Generic;
using Godot;

namespace ZooBuilder.globals.saveable;

public record InventorySlotDto
{
    public int Index { get; init; }
    public string ItemName { get; init; }
    public int Amount { get; init; }

    public static List<InventorySlotResource> AsInventorySlotResource(List<InventorySlotDto> inventorySlotDtoList, int inventorySize)
    {
        List<InventorySlotResource> inventory = new(inventorySize);

        Dictionary<int, InventorySlotDto> indexSlotDtoMap = new();
        if (inventorySlotDtoList != null)
        {
            foreach (InventorySlotDto slotDto in inventorySlotDtoList)
            {
                indexSlotDtoMap.Add(slotDto.Index, slotDto);
            }
        }

        for (int i = 0; i < inventorySize; i++)
        {
            if (indexSlotDtoMap.TryGetValue(i, out InventorySlotDto inventorySlotsDto))
            {
                string itemName = inventorySlotsDto.ItemName;
                int amount = inventorySlotsDto.Amount;
                inventory.Add(new InventorySlotResource(i, new ItemStackResource(ItemDatabase.Get(itemName), amount)));
            }
            else
            {
                inventory.Add(new InventorySlotResource(i, null));
            }
        }

        return inventory;

        // TODO: If inventorySize says 48, but saved inventory only has 24 items, then capacity will be set to 48, but count to 24, and inventory[>23] will fail.
        // Consider using array.. But what about limitless chests?
        // Maybe just fill in the rest with empty InventorySlotResource?
        // Or, maybe just don't store the inventory size as a separate field? Or can we assume that inventorySize always == inventory.count in DTO?
        // Or maybe just deduce int inventorySize from inventory.count in DTO?
        // Maybe store the index for each itemslot dto? These can then be stored in a map and it would be easy to know for which indexes to use saved data, and for which to make empty slotResource

        // TODO: Have a proper list of ItemDataResources. Maybe load them as a map somehow, by ItemName? Make ItemName an enum?
    }

    public static List<InventorySlotDto> FromInventorySlotResource(List<InventorySlotResource> inventorySlotResourceList)
    {
        int capacity = inventorySlotResourceList.Capacity;
        List<InventorySlotDto> inventory = new(capacity);
        for (int i = 0; i < capacity; i++)
        {
            InventorySlotResource slot = inventorySlotResourceList[i];
            if (slot.HasItem())
            {
                inventory.Add(new InventorySlotDto
                {
                    Index = i,
                    ItemName = slot.GetItem().ItemData.ItemName,
                    Amount = slot.GetItem().Amount
                });
            }
        }

        return inventory;
    }
}
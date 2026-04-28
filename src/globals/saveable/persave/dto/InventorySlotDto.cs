using System.Collections.Generic;
using Godot;

namespace ZooBuilder.globals.saveable;

public record InventorySlotDto
{
    public string ItemName { get; init; }
    public int Amount { get; init; }

    public static List<InventorySlotResource> AsInventorySlotResource(List<InventorySlotDto> inventorySlotDtoList, int inventorySize)
    {
        List<InventorySlotResource> inventory = new(inventorySize);
        if (inventorySlotDtoList == null)
        {
            for (int i = 0; i < inventorySize; i++)
            {
                inventory.Add(new InventorySlotResource(i, null));
            }
            return inventory;
        }

        // TODO: If inventorySize says 48, but saved inventory only has 24 items, then capacity will be set to 48, but count to 24, and inventory[>23] will fail.
        // Consider using array.. But what about limitless chests?
        // Maybe just fill in the rest with empty InventorySlotResource?
        // Or, maybe just don't store the inventory size as a separate field? Or can we assume that inventorySize always == inventory.count in DTO?
        // Or maybe just deduce int inventorySize from inventory.count in DTO?
        
        // TODO: Have a proper list of ItemDataResources. Maybe load them as a map somehow, by ItemName? Make ItemName an enum?
        foreach (InventorySlotDto inventorySlotDto in inventorySlotDtoList)
        {
            if (inventorySlotDto == null)
            {
                inventory.Add(new InventorySlotResource(1, null));
                continue;
            }

            string itemName = inventorySlotDto.ItemName;
            int amount = inventorySlotDto.Amount;
            inventory.Add(new InventorySlotResource(1, new ItemStackResource(new ItemDataResource { ItemName = itemName, Description = "What's up" }, amount)));
        }

        GD.Print("INVENTORY STUFF: " + inventorySize + " " + inventory.Capacity);
        return inventory;
    }

    public static List<InventorySlotDto> FromInventorySlotResource(List<InventorySlotResource> inventorySlotResourceList)
    {
        List<InventorySlotDto> inventory = [];
        foreach (InventorySlotResource slot in inventorySlotResourceList)
        {
            if (slot.IsEmpty())
            {
                inventory.Add(null);
            }
            else
                inventory.Add(new InventorySlotDto
                {
                    ItemName = slot.GetItem().ItemData.ItemName,
                    Amount = slot.GetItem().Amount
                });
        }

        return inventory;
    }
}
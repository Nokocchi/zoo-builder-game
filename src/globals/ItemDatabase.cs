using System.Collections.Generic;
using Godot;

namespace ZooBuilder.globals;

public static class ItemDatabase
{
    public static readonly Dictionary<string, ItemDataResource> Items = new();

    public static void Initialize()
    {
        // Load all resources from a folder
        DirAccess dir = DirAccess.Open("res://data/resources/items");
        if (dir == null)
        {
            GD.PrintErr("Failed to open Items directory");
            return;
        }

        dir.ListDirBegin();
        string fileName;

        while ((fileName = dir.GetNext()) != "")
        {
            if (dir.CurrentIsDir())
                continue;

            if (!fileName.EndsWith(".tres"))
                continue;

            string path = $"res://data/resources/items/{fileName}";
            ItemDataResource resource = ResourceLoader.Load<ItemDataResource>(path);

            if (resource == null)
                continue;

            if (Items.ContainsKey(resource.ItemName))
            {
                GD.PrintErr($"Duplicate item name: {resource.ItemName}");
                continue;
            }

            Items[resource.ItemName] = resource;
        }

        dir.ListDirEnd();
    }

    public static ItemDataResource Get(string id)
    {
        if (Items.TryGetValue(id, out var item))
            return item;

        GD.PrintErr($"Item not found: {id}");
        return null;
    }
}
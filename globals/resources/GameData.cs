using System;
using Godot;
using Godot.Collections;

namespace ZooBuilder.globals.resources;

[GlobalClass]
public partial class GameData : Resource
{
    private const string SaveFileLocation = "user://GameData.tres";
    private const string SaveFileLocationTemplate = "user://GameData.tres";

    // TODO: Use ConfigFile?? 
    [Export] public int InventorySize = 24;
    [Export] public Vector3 PlayerGlobalPosition = Vector3.Zero;
    [Export] public Vector3 PlayerRotation = Vector3.Zero;
    [Export] public Vector3 CameraRotation = Vector3.Zero;
    [Export] public int GameTime;
    [Export] public Array<ItemStackResource> Inventory;

    public static GameData Load()
    {
        bool fileExists = ResourceLoader.Exists(SaveFileLocation);
        if (!fileExists)
        {
            new GameData().Save();
        }

        return ResourceLoader.Load<GameData>("user://GameData.tres");
    }

    public void Save()
    {
        ResourceSaver.Save(this, "user://GameData.tres");
    }
}
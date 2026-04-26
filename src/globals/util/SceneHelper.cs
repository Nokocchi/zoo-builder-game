using Godot;

namespace ZooBuilder.globals.util;

public class SceneHelper
{
    public void PrintNodeHierarchy(Node fomNode)
    {
        GD.Print($"[SettingsCategoryContainer] Ready: {fomNode.GetPath()}");

        Node parent = fomNode.GetParent();
        while (parent != null)
        {
            GD.Print($"  Parent: {parent.Name} ({parent.GetType().Name})");
            parent = parent.GetParent();
        }

        GD.Print($"  Scene file: {fomNode.SceneFilePath}");
    }
}
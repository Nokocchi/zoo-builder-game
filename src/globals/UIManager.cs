using System.Collections.Generic;

namespace ZooBuilder.globals;

public class UIManager
{
    public static List<UILayer> layers
    {
        get;
    } = [UILayer.HUD];
    
    public enum UILayer
    {
        HUD,
        MENU
    }

    public static bool IsMenuOpen()
    {
        return layers.Contains(UILayer.MENU);
    }

    public static void CloseMenu()
    {
        layers.Remove(UILayer.MENU);
    }

    public static void OpenMenu()
    {
        layers.Add(UILayer.MENU);
    }
}
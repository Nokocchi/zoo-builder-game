using System.Linq;
using Godot;
using ZooBuilder.globals;
using ZooBuilder.ui.inventory;

public partial class Menu : Control
{
    private TabContainer _tabContainer;

    private int _inventoryTabIndex;
    private int _settingsTabIndex;
    private int _achievementsTabIndex;
    private MainInventoryUI _inventory;
    private Settings _settings;
    private AchievementsUI _achievements;
    
    private static readonly PackedScene InventoryScene = GD.Load<PackedScene>("res://src/ui/ingamemenu/inventory/inventory.tscn");
    private static readonly PackedScene SettingsScene = GD.Load<PackedScene>("res://src/ui/ingamemenu/settings/settings.tscn");
    private static readonly PackedScene AchievementsScene = GD.Load<PackedScene>("res://src/ui/ingamemenu/achievement/achievements.tscn");

    public override void _Ready()
    {
        _tabContainer = GetNode<TabContainer>("%TabContainer");
        _inventory = InventoryScene.Instantiate<MainInventoryUI>();
        _settings = SettingsScene.Instantiate<Settings>();
        _achievements = AchievementsScene.Instantiate<AchievementsUI>();
        _inventoryTabIndex = AddTab(_inventory, "MENU_TAB_INVENTORY");
        _settingsTabIndex = AddTab(_settings, "MENU_TAB_SETTINGS");
        _achievementsTabIndex = AddTab(_achievements, "MENU_TAB_ACHIEVEMENTS");
        Visible = false;
    }
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed(GlobalDataSingleton.ACTION_OPEN_INVENTORY))
            HandleMenuOpenButton(MenuTab.Inventory);

        if (@event.IsActionPressed(GlobalDataSingleton.ACTION_OPEN_SETTINGS))
            HandleMenuOpenButton(MenuTab.Settings);

        if (@event.IsActionPressed(GlobalDataSingleton.ACTION_OPEN_ACHIEVEMENTS))
            HandleMenuOpenButton(MenuTab.Achievements);
    }

    private void HandleMenuOpenButton(MenuTab tabToShow)
    {
        if (UIManager.IsMenuOpen())
        {
            int currentTabIndex = _tabContainer.GetCurrentTab();
            int indexOfTabToShow = GetTabIndex(tabToShow);
            if (currentTabIndex == indexOfTabToShow)
            {
                Visible = false;
                UIManager.CloseMenu();
                InventorySingleton.Instance.TossEntireHeldItemStack();
            }
            else
            {
                _tabContainer.SetCurrentTab(indexOfTabToShow);
            }
        }
        else
        {
            _settings.Initialize();
            UIManager.OpenMenu();
            Visible = true;
            _tabContainer.SetCurrentTab(GetTabIndex(tabToShow));
        }
        
        Input.MouseMode = UIManager.IsMenuOpen() ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
    }
    
    private int AddTab(Control content, string title)
    {
        MenuTabContainerPanel panel = MenuTabContainerPanel.CreateWithContent(content);
        _tabContainer.AddChild(panel);

        int index = _tabContainer.GetTabIdxFromControl(panel);
        _tabContainer.SetTabTitle(index, title);

        return index;
    }
    
    private void TabContainerTabSelectionChangedListener(int newTabIndex)
    {
        if (newTabIndex == _inventoryTabIndex)
        {
        } else if (newTabIndex == _settingsTabIndex)
        {
        } else if (newTabIndex == _achievementsTabIndex)
        {
        }
    }

    private int GetTabIndex(MenuTab tab)
    {
        return tab switch
        {
            MenuTab.Inventory => _inventoryTabIndex,
            MenuTab.Settings => _settingsTabIndex,
            MenuTab.Achievements => _achievementsTabIndex,
            _ => 0
        };
    }
    
    private enum MenuTab
    {
        Inventory,
        Settings,
        Achievements
    }

}

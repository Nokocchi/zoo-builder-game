using System.Collections.Generic;
using System.Linq;
using Godot;
using ZooBuilder.globals;
using ZooBuilder.globals.saveable;
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
    private SaveFileList _saveFileList;
    
    private static readonly PackedScene InventoryScene = GD.Load<PackedScene>("res://src/ui/ingamemenu/inventory/inventory.tscn");
    private static readonly PackedScene SettingsScene = GD.Load<PackedScene>("res://src/ui/ingamemenu/settings/settings.tscn");
    private static readonly PackedScene AchievementsScene = GD.Load<PackedScene>("res://src/ui/ingamemenu/achievement/achievements.tscn");
    private static readonly PackedScene SaveFileListScene = GD.Load<PackedScene>("res://src/ui/shared/SaveFileList.tscn");
    private static readonly PackedScene MainScene = GD.Load<PackedScene>("res://src/main.tscn");

    public override void _Ready()
    {
        _tabContainer = GetNode<TabContainer>("%TabContainer");
        _inventory = InventoryScene.Instantiate<MainInventoryUI>();
        _settings = SettingsScene.Instantiate<Settings>();
        _achievements = AchievementsScene.Instantiate<AchievementsUI>();
        _inventoryTabIndex = AddTab(_inventory, "MENU_TAB_INVENTORY");
        _settingsTabIndex = AddTab(_settings, "MENU_TAB_SETTINGS");
        _achievementsTabIndex = AddTab(_achievements, "MENU_TAB_ACHIEVEMENTS");
        _saveFileList = SaveFileListScene.Instantiate<SaveFileList>();
        AddTab(_saveFileList, "Saves");
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
            SortedList<long, GameData> sortedListOfSaves = GameDataSingleton.GetSortedListOfSaves();
            _saveFileList.SetSaveFiles(sortedListOfSaves, OnSelectedSaveFile);
            UIManager.OpenMenu();
            Visible = true;
            _tabContainer.SetCurrentTab(GetTabIndex(tabToShow));
        }
        
        Input.MouseMode = UIManager.IsMenuOpen() ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
    }
    
    private void OnSelectedSaveFile(GameData selectedSaveFile)
    {
        GameDataSingleton.SetLoadedSaveFile(selectedSaveFile);
        GetTree().ChangeSceneToPacked(MainScene);
        UIManager.CloseMenu(); // Not a node within the scene, so state is not reset automatically. IsMenuOpen would be true even after game reload, blocking player movement input.
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

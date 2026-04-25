using System.Linq;
using Godot;
using ZooBuilder.globals;
using ZooBuilder.ui.inventory;

public partial class Menu : Control
{
    private TabContainer _tabContainer;

    private MainInventoryGridContainer _inventory;
    private Settings _settings;
    private AchievementsUI _achievements;
    
    // TODO: This seems a bit hacky. Could it be automated somehow? 
    private const int INVENTORY_TAB_INDEX = 0;
    private const int SETTINGS_TAB_INDEX = 1;
    private const int ACHIEVEMENTS_TAB_INDEX = 2;

    public override void _Ready()
    {
        _tabContainer = GetNode<TabContainer>("%TabContainer");
        _inventory = (MainInventoryGridContainer)_tabContainer.GetTabControl(INVENTORY_TAB_INDEX);
        _settings = (Settings)_tabContainer.GetTabControl(SETTINGS_TAB_INDEX);
        _achievements = (AchievementsUI)_tabContainer.GetTabControl(ACHIEVEMENTS_TAB_INDEX);
        Visible = false;
    }
    
    public override void _Input(InputEvent @event)
    {
        bool anyRemapButtonsListening = GetTree()
            .GetNodesInGroup(InputRemapButton.INPUT_REMAP_BUTTON_GROUP_NAME)
            .OfType<InputRemapButton>()
            .Any(button => button.IsPressed());
        
        if (anyRemapButtonsListening) return;
        
        if(@event.IsActionPressed(GlobalDataSingleton.ACTION_OPEN_INVENTORY))
        {
            HandleMenuOpenButton(INVENTORY_TAB_INDEX);
        }
        if(@event.IsActionPressed(GlobalDataSingleton.ACTION_OPEN_SETTINGS))
        {
            // TODO: Do this whenever settings are opened, not just when "O" is pressed
            _settings.Initialize();
            HandleMenuOpenButton(SETTINGS_TAB_INDEX);
        }
        if(@event.IsActionPressed(GlobalDataSingleton.ACTION_OPEN_ACHIEVEMENTS))
        {
            HandleMenuOpenButton(ACHIEVEMENTS_TAB_INDEX);
        }
    }

    private void HandleMenuOpenButton(int tabIndex)
    {
        int currentTabIndex = _tabContainer.GetCurrentTab();
        if (UIManager.IsMenuOpen())
        {
            if (currentTabIndex == tabIndex)
            {
                Visible = false;
                UIManager.CloseMenu();
                InventorySingleton.Instance.TossEntireHeldItemStack();
            }
            else
            {
                _tabContainer.SetCurrentTab(tabIndex);
            }
        }
        else
        {
            UIManager.OpenMenu();
            Visible = true;
            _tabContainer.SetCurrentTab(tabIndex);
        }
        Input.MouseMode = Visible ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
    }

    // TODO: Can this be used for anything? 
    private void TabContainerTabSelectionChangedListener(int newTabIndex)
    {
        if (newTabIndex == INVENTORY_TAB_INDEX)
        {
        } else if (newTabIndex == SETTINGS_TAB_INDEX)
        {
        } else if (newTabIndex == ACHIEVEMENTS_TAB_INDEX)
        {
        }
    }


}

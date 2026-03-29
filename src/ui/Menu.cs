using Godot;
using ZooBuilder.globals;

public partial class Menu : Control
{
    private TabContainer _tabContainer;

    private const int INVENTORY_TAB_INDEX = 0;
    private const int SETTINGS_TAB_INDEX = 1;
    private const int ACHIEVEMENTS_TAB_INDEX = 2;

    public override void _Ready()
    {
        _tabContainer = GetNode<TabContainer>("%TabContainer");
        Visible = false;
    }
    
    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionPressed(ActionConstants.OPEN_INVENTORY))
        {
            HandleMenuOpenButton(INVENTORY_TAB_INDEX);
        }
        if(@event.IsActionPressed(ActionConstants.OPEN_SETTINGS))
        {
            HandleMenuOpenButton(SETTINGS_TAB_INDEX);
        }
        if(@event.IsActionPressed(ActionConstants.OPEN_ACHIEVEMENTS))
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


}

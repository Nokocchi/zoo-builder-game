using Godot;
using System;

public partial class MenuTabContainerPanel : Control
{
	
	private static readonly PackedScene MenuTabContainerPanelScene = GD.Load<PackedScene>("res://src/ui/ingamemenu/menu_tab_container_panel.tscn");
	private ScrollContainer _scrollContainer;
	private Node _content;

	public override void _Ready()
	{
		_scrollContainer = GetNode<ScrollContainer>("%ScrollContainer");
		_scrollContainer.AddChild(_content);
	}

	public static MenuTabContainerPanel CreateWithContent(Node content)
	{
		MenuTabContainerPanel menuTabContainerPanel = MenuTabContainerPanelScene.Instantiate<MenuTabContainerPanel>();
		menuTabContainerPanel._content = content;
		return menuTabContainerPanel;
	}
}

using Godot;
using IdleFramework.Global;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// [IdleFramework内置UI场景-控件主题]根节点
/// </summary>
[GlobalClass]
public partial class Control : UIScene , IClassPackedScene
{
	public PackedScene CPS
	{
		get
		{
			field ??= GD.Load<PackedScene>("res://addons/idle_framework/ui_scenes/control/ui_scene_control.tscn");
			return field;
		}
	}

	public TextureRect NTopBar_Icon;
	public Label NTopBar_GameTitle;
	public HBoxContainer NTopBar_PinnedItemsBar;
	public VBoxContainer NMainSpace_SpaceButtons;
	public PanelContainer NMainSpace_DetailArea;
	
	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationSceneInstantiated:
				NTopBar_Icon = GetNode<TextureRect>("VBC/TopBar/VBC/Icon");
				NTopBar_GameTitle = GetNode<Label>("VBC/TopBar/VBC/Name");
				NTopBar_PinnedItemsBar = GetNode<HBoxContainer>("VBC/TopBar/VBC/PinnedItemsBar");
				NMainSpace_SpaceButtons = GetNode<VBoxContainer>("VBC/MainSpace/SideBar/SC/SpaceButtons");
				NMainSpace_DetailArea = GetNode<PanelContainer>("VBC/MainSpace/DetailArea");
				break;
		}
	}

	public override void _Ready()
	{
		NTopBar_GameTitle.Text = Localization.Tr(MotherNode.GameResource.NameKey);
	}
}

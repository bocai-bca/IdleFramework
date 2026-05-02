using Godot;
using IdleFramework.Global;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// [IdleFramework内置UI场景-控件主题]置顶物品
/// </summary>
[GlobalClass]
public partial class PinnedItem : PanelContainer, IClassPackedScene
{
	public static PackedScene CPS => field ??= GD.Load<PackedScene>("res://addons/idle_framework/ui_scenes/control/pinned_item/pinned_item.tscn");
	
	public TextureRect NIcon;

	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationSceneInstantiated:
				NIcon = GetNode<TextureRect>("MC/HBC/Icon");
				break;
		}
	}
}

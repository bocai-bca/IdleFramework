using Godot;
using IdleFramework.Global;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// [IdleFramework内置UI场景-控件主题]容器物品小按钮，在容器实例展开式容器中显示单个物品的图标和数量的按钮
/// </summary>
[GlobalClass]
public partial class ContainerItem : Button, IClassPackedScene
{
	public static PackedScene CPS => field ??= GD.Load<PackedScene>("res://addons/idle_framework/ui_scenes/control/container_item/container_item.tscn");
	
	/// <summary>
	/// 设置物品图标。
	/// </summary>
	/// <param name="itemIcon">物品图标。</param>
	public void SetItemIcon(Texture2D itemIcon) => Icon = itemIcon;
	
	/// <summary>
	/// 设置物品数量。
	/// </summary>
	/// <param name="itemCount">物品数量。</param>
	public void SetItemCount(long itemCount) => Text = itemCount.ToString();
}

using Godot;
using IdleFramework.Global;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// [IdleFramework内置UI场景-控件主题]空间详细区域容器，在详细区域面板(DetailArea)显示的代表一个空间的所有内容的面板
/// </summary>
[GlobalClass]
public partial class SpaceDetailArea : ScrollContainer, IClassPackedScene
{
	public static PackedScene CPS => field ??= GD.Load<PackedScene>("res://addons/idle_framework/ui_scenes/control/space_detail_area/space_detail_area.tscn");
}

using Godot;
using IdleFramework.Global;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// [IdleFramework内置UI场景-控件主题]空间按钮，在侧边栏上显示所有空间按钮，按下后跳转到该空间
/// </summary>
[GlobalClass]
public partial class SpaceButton : Button, IClassPackedScene
{
	public static PackedScene CPS => field ??= GD.Load<PackedScene>("res://addons/idle_framework/ui_scenes/control/space_button/space_button.tscn");
	/// <summary>
	/// 对应的空间ID
	/// </summary>
	public string SpaceID { get; set; }
}

#if IDLE_FRAMEWORK_UISCENE_ALL || IDLE_FRAMEWORK_UISCENE_CONTROL
using Godot;
using IdleFramework.Global;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// [IdleFramework内置UI场景-控件主题]物品实例展开式容器编辑按钮，在物品实例展开式容器的标题区域呈现的编辑按钮
/// </summary>
[GlobalClass]
public partial class ItemContainerEditButton : Button, IClassPackedScene
{
	public static PackedScene CPS => field ??= GD.Load<PackedScene>("res://addons/idle_framework/ui_scenes/control/item_container_edit_button/item_container_edit_button.tscn");
}
#endif
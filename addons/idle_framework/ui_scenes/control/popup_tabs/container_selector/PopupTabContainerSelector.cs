#if IDLE_FRAMEWORK_UISCENE_ALL || IDLE_FRAMEWORK_UISCENE_CONTROL
using System;
using Godot;
using IdleFramework.Core;
using IdleFramework.Global;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// [IdleFramework内置UI场景-控件主题]容器选择弹窗标签页，用于显示一个工厂的输入或输出容器的可选列表并供玩家选择切换，同时提供容器编组的制作和管理。
/// </summary>
/// <remarks>此类标签页名称的规范模板为：<c>RecipeSelector.{factoryGuid}</c>。</remarks>
[GlobalClass]
public partial class PopupTabContainerSelector : PopupTabBase, IClassPackedScene
{
	public static PackedScene CPS => field ??= GD.Load<PackedScene>("res://addons/idle_framework/ui_scenes/control/popup_tabs/container_selector/popup_tab_container_selector.tscn");

	/// <summary>
	/// 对对应工厂GUID的保存，用于读写数据。需要由创建本类实例的对象负责赋值，否则实例将无法正常运作
	/// </summary>
	public Guid factoryGuidCache = Guid.Empty;

	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationSceneInstantiated:
				
				break;
		}
	}
	public override string GetTitleName()
	{
		return string.Format(Localization.Tr("ui_scene_control.popup_tab_title.container_selector"), SaveAccess.LoadedDataHelper.GetNameForInstance(factoryGuidCache, "ui_scene_control.lost_instance"));
	}

	public override void Update(SaveDataHelper saveDataHelper)
	{
		
	}

}
#endif
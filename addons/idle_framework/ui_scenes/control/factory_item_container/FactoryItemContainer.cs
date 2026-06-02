#if IDLE_FRAMEWORK_UISCENE_ALL || IDLE_FRAMEWORK_UISCENE_CONTROL
using System;
using System.Collections.Generic;
using Godot;
using IdleFramework.Core;
using IdleFramework.Global;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// [IdleFramework内置UI场景-控件主题]工厂实例展开式容器，在空间详细区域容器中显示一个工厂实例
/// </summary>
[GlobalClass]
public partial class FactoryItemContainer : FoldableContainer, IClassPackedScene
{
	public static PackedScene CPS => field ??= GD.Load<PackedScene>("res://addons/idle_framework/ui_scenes/control/factory_item_container/factory_item_container.tscn");
	
	public Button NEditButton;
	
	/// <summary>
	/// 对应的工厂实例的GUID
	/// </summary>
	public Guid FactoryGuid { get; set; }
	
	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationSceneInstantiated:
				NEditButton = ItemContainerEditButton.Create();
				NEditButton.Text = Localization.Tr("ui_scene_control.edit_button");
				AddTitleBarControl(NEditButton);
				break;
		}
	}
	
	/// <summary>
	/// 设置该工厂实例展开式容器的标题名，应对应其物品实例的名称
	/// </summary>
	/// <param name="titleName">要设置为的标题名。</param>
	public void SetTitleName(string titleName) => Title = titleName;
	
	/// <summary>
	/// 完全更新，进行更全面的内容更新。
	/// </summary>
	/// <param name="saveDataHelper">要使用的存档数据辅助器。</param>
	public void FullyUpdate(SaveDataHelper saveDataHelper)
	{
		SetTitleName(saveDataHelper.GetNameForInstance(FactoryGuid));
		Update(saveDataHelper);
	}
	
	/// <summary>
	/// 更新。
	/// </summary>
	/// <param name="saveDataHelper">要使用的存档数据辅助器。</param>
	public void Update(SaveDataHelper saveDataHelper)
	{
		if (false /*TODO 这里可以通过从SaveDataHelper获取工厂实例的数据，来一举两得完成对工厂实例是否存在的检测和取得用来反映到场景节点的数据*/)
		{
			Logger.LogInfo(Localization.Tr("log.info.ui_scene_control_factory_item_container.going_to_remove_because_corresponding_guid_is_not_exist_anymore"));
			QueueFree();
			return;
		}
	}
}
#endif
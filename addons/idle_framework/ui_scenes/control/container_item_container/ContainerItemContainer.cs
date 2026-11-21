#if IDLE_FRAMEWORK_UISCENE_ALL || IDLE_FRAMEWORK_UISCENE_CONTROL
using System;
using System.Collections.Generic;
using Godot;
using IdleFramework.Core;
using IdleFramework.Global;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// [IdleFramework内置UI场景-控件主题]容器实例展开式容器，在空间详细区域容器中显示一个容器实例的内容物
/// </summary>
[GlobalClass]
public partial class ContainerItemContainer : FoldableContainer, IClassPackedScene
{
	public static PackedScene CPS => field ??= GD.Load<PackedScene>("res://addons/idle_framework/ui_scenes/control/container_item_container/container_item_container.tscn");

	public HFlowContainer NItems;
	public Button NEditButton;
	public readonly Dictionary<string, ContainerItem> NContainerItems = [];
	
	/// <summary>
	/// 对应的容器实例的GUID
	/// </summary>
	public Guid ContainerGuid { get; set; }

	public override void _Notification(int what)
	{
		switch (what)
		{
			case (int)NotificationSceneInstantiated:
				NItems = GetNode<HFlowContainer>("Items");
				NEditButton = ItemContainerEditButton.Create();
				NEditButton.Text = Localization.Tr("ui_scene_control.edit_button");
				AddTitleBarControl(NEditButton);
				break;
		}
	}

	/// <summary>
	/// 设置该容器实例展开式容器的标题名，应对应其物品实例的名称
	/// </summary>
	/// <param name="titleName">要设置为的标题名。</param>
	public void SetTitleName(string titleName) => Title = titleName;

	/// <summary>
	/// 完全更新，进行更全面的内容更新。
	/// </summary>
	/// <param name="saveDataHelper">要使用的存档数据辅助器。</param>
	public void FullyUpdate(SaveDataHelper saveDataHelper)
	{
		SetTitleName(saveDataHelper.GetNameForInstance(ContainerGuid));
		Update(saveDataHelper);
	}
	
	/// <summary>
	/// 更新。
	/// </summary>
	/// <param name="saveDataHelper">要使用的存档数据辅助器。</param>
	public void Update(SaveDataHelper saveDataHelper)
	{
		if (!saveDataHelper.GetAllItemCountsForContainer(ContainerGuid, out Dictionary<string, long> itemCounts))
		{
			Logger.LogInfo(Localization.Tr("log.info.ui_scene_control_container_item_container.going_to_remove_because_corresponding_guid_is_not_exist_anymore"));
			QueueFree();
			return;
		}
		foreach ((string itemId, _) in itemCounts)
		{
			if (NContainerItems.ContainsKey(itemId)) continue;
			ContainerItem containerItem = ContainerItem.Create();
			if (saveDataHelper.UsingGameResource.ItemRegistry.TryGetValue(itemId, out ItemRegistryObject itemRegistryObject))
			{
				containerItem.itemNameCache = Localization.Tr(itemRegistryObject.NameKey);
				containerItem.itemIdCache = itemId;
				containerItem.SetItemIcon(itemRegistryObject.IconTexture);
			}
			NContainerItems[itemId] = containerItem;
			NItems.AddChild(containerItem);
		}
		List<string> freeItemIds = [];
		foreach ((string containerItemId, ContainerItem containerItem) in NContainerItems)
		{
			if (!itemCounts.TryGetValue(containerItemId, out long itemCount))
			{
				freeItemIds.Add(containerItemId);
				continue;
			}
			containerItem.SetItemCount(itemCount);
		}
		foreach (string freeItemId in freeItemIds)
		{
			NContainerItems[freeItemId].QueueFree();
			NContainerItems.Remove(freeItemId);
		}
	}
}
#endif
using System;
using System.Collections.Generic;
using Godot;
using IdleFramework.Core;
using IdleFramework.Global;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// [IdleFramework内置UI场景-控件主题]空间详细区域容器，在详细区域面板(DetailArea)显示的代表一个空间的所有内容的面板
/// </summary>
[GlobalClass]
public partial class SpaceDetailArea : ScrollContainer, IClassPackedScene
{
	public static PackedScene CPS => field ??= GD.Load<PackedScene>("res://addons/idle_framework/ui_scenes/control/space_detail_area/space_detail_area.tscn");

	public VBoxContainer NVBC;
	public ContainerItemContainer NSpaceContainerItemContainer;
	public readonly Dictionary<Guid, ContainerItemContainer> NContainerItemContainers = [];

	/// <summary>
	/// 对应的空间ID
	/// </summary>
	public string SpaceId { get; set; }
	
	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationSceneInstantiated:
				NVBC = GetNode<VBoxContainer>("VBC");
				break;
		}
	}
	
	/// <summary>
	/// 更新详细区域容器。
	/// </summary>
	/// <param name="saveDataHelper">要使用的存档数据辅助器。</param>
	public void Update(SaveDataHelper saveDataHelper)
	{
		CollectAndPlaceElements(saveDataHelper);
		NSpaceContainerItemContainer?.Update(saveDataHelper);
		foreach (ContainerItemContainer containerItemContainer in NContainerItemContainers.Values)
		{
			containerItemContainer.Update(saveDataHelper);
		}
	}

	/// <summary>
	/// 收集并放置元素，从存档数据中寻找目前场景中不存在的内容并添加进去。
	/// </summary>
	/// <param name="saveDataHelper">要使用的存档数据辅助器。</param>
	public void CollectAndPlaceElements(SaveDataHelper saveDataHelper)
	{
		if (!saveDataHelper.GetAllInstanceGuidsInSpace(SpaceId, out Dictionary<string, List<Guid>> instanceItemGuids))
		{
			Logger.LogError(Localization.Tr("log.error.ui_scene_control_space_detail_area.failed_to_get_all_instance_guids_for_space_id_owned"));
			return;
		}
		GameResource gameResource = MotherNode.Instance.GameResource;
		foreach ((string itemId, List<Guid> itemGuids) in instanceItemGuids)
		{
			if (gameResource.IsContainer(itemId))
			{
				foreach (Guid itemGuid in itemGuids)
				{
					if (NContainerItemContainers.ContainsKey(itemGuid)) continue;
					Logger.LogInfo(Localization.Tr("log.info.ui_scene_control_space_detail_area.adding_container_item_container_instance"));
					ContainerItemContainer containerItemContainer = ContainerItemContainer.Create();
					containerItemContainer.ContainerGuid = itemGuid;
					NContainerItemContainers[itemGuid] = containerItemContainer;
					NVBC.AddChild(containerItemContainer);
				}
			}
			if (gameResource.IsFactory(itemId))
			{
				
			}
		}
		if (NSpaceContainerItemContainer == null && saveDataHelper.GetSpaceContainerGuidForSpace(SpaceId, out Guid spaceContainerGuid))
		{
			ContainerItemContainer spaceContainerItemContainer = ContainerItemContainer.Create();
			spaceContainerItemContainer.ContainerGuid = spaceContainerGuid;
			spaceContainerItemContainer.Name = new StringName("SpaceContainer");
			saveDataHelper.SetNameForInstance(spaceContainerGuid, string.Format(Localization.Tr("ui_scene_control.space_container_name"), gameResource.GetSpaceNameTranslated(SpaceId)));
			NSpaceContainerItemContainer = spaceContainerItemContainer;
			NVBC.AddChild(spaceContainerItemContainer);
			spaceContainerItemContainer.FullyUpdate(saveDataHelper);
		}
	}
}

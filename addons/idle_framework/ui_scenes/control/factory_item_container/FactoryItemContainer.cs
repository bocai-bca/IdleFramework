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
	public Label NProgressTimeText;
	public ProgressBar NProgressProgressBar;
	public Label NProgressStandbyText;
	public Button NInputContainerButton;
	public Button NOutputContainerButton;
	public Button NRecipeButton;
	
	/// <summary>
	/// 对应的工厂实例的GUID
	/// </summary>
	public Guid FactoryGuid { get; set; }

	public FactoryRegistryObject FactoryRegistryObjectCache;
	public Guid InputContainerGuidCache = Guid.Empty;
	public Guid OutputContainerGuidCache = Guid.Empty;
	
	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationSceneInstantiated:
				NProgressTimeText = GetNode<Label>("VBC/HBC/Progress/TimeText");
				NProgressProgressBar = GetNode<ProgressBar>("VBC/HBC/Progress/ProgressBar");
				NProgressStandbyText = GetNode<Label>("VBC/HBC/Progress/StandbyText");
				NProgressStandbyText.Text = Localization.Tr("ui_scene_control.standby");
				NEditButton = ItemContainerEditButton.Create();
				NEditButton.Text = Localization.Tr("ui_scene_control.edit_button");
				AddTitleBarControl(NEditButton);
				NInputContainerButton = GetNode<Button>("VBC/HBC/InputContainerButton");
				NInputContainerButton.TooltipText = Localization.Tr("ui_scene_control.click_to_set_container");
				NOutputContainerButton = GetNode<Button>("VBC/HBC/OutputContainerButton");
				NOutputContainerButton.TooltipText = Localization.Tr("ui_scene_control.click_to_set_container");
				NRecipeButton = GetNode<Button>("VBC/RecipeBar/RecipeButton");
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
		if (!saveDataHelper.GetFactoryForGuid(FactoryGuid, out FactoryData factoryData))
		{
			Logger.LogInfo(Localization.Tr("log.info.ui_scene_control_factory_item_container.going_to_remove_because_corresponding_guid_is_not_exist_anymore"));
			QueueFree();
			return;
		}
		if (factoryData.CurrentRecipe == string.Empty)
		{
			NProgressStandbyText.Visible = true;
			NProgressTimeText.Visible = NProgressProgressBar.Visible = false;
		}
		else
		{
			NProgressStandbyText.Visible = false;
			NProgressTimeText.Visible = NProgressProgressBar.Visible = true;
		}
		if (string.IsNullOrEmpty(factoryData.FactoryIdCache))
		{
			NRecipeButton.Disabled = true;
			NRecipeButton.Icon = null;
			NRecipeButton.Text = Localization.Tr("ui_scene_control.waiting_for_update");
		}
		else
		{
			if (!saveDataHelper.UsingGameResource.RecipeRegistry.TryGetValue(factoryData.CurrentRecipe, out RecipeRegistryObject recipeRegistryObject))
			{
				Logger.LogError(Localization.Tr("log.error.ui_scene_control_factory_item_container.failed_to_get_recipe_registry_object_for_current_recipe"));
			}
			else
			{
				UpdateRecipeBar(saveDataHelper, factoryData, recipeRegistryObject);
			}
		}
		UpdateContainerButton(saveDataHelper, NInputContainerButton, factoryData.InputContainerGuid, InputContainerGuidCache);
		UpdateContainerButton(saveDataHelper, NOutputContainerButton, factoryData.OutputContainerGuid, OutputContainerGuidCache);
	}

	public static void UpdateContainerButton(SaveDataHelper saveDataHelper, Button buttonNode, Guid containerGuidCurrent, Guid containerGuidCache)
	{
		if (!saveDataHelper.IsContainerMixinExistsForGuid(containerGuidCurrent))
		{
			buttonNode.Icon = null;
			buttonNode.Text = "--";
		}
		else
		{
			if (containerGuidCurrent == containerGuidCache) return;
			containerGuidCurrent = containerGuidCache;
			string itemIdForGuid = saveDataHelper.QueryItemIdForGuid(containerGuidCurrent);
			buttonNode.Icon = itemIdForGuid != string.Empty && saveDataHelper.UsingGameResource.ItemRegistry.TryGetValue(itemIdForGuid, out ItemRegistryObject itemRegistryObject) ? itemRegistryObject.IconTexture : null;
			buttonNode.Text = saveDataHelper.GetNameForInstance(containerGuidCurrent);
		}
	}

	public void UpdateRecipeBar(SaveDataHelper saveDataHelper, FactoryData factoryData, RecipeRegistryObject recipeRegistryObject)
	{
		
	}
}
#endif
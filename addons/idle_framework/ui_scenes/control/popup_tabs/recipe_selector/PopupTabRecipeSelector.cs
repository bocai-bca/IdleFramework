#if IDLE_FRAMEWORK_UISCENE_ALL || IDLE_FRAMEWORK_UISCENE_CONTROL
using System;
using System.Collections.Generic;
using Godot;
using IdleFramework.Core;
using IdleFramework.Global;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// [IdleFramework内置UI场景-控件主题]配方选择弹窗标签页，用于显示一个工厂的可选配方并供玩家浏览和选择要制作的物品。
/// </summary>
[GlobalClass]
public partial class PopupTabRecipeSelector : PopupTabBase, IClassPackedScene
{
	public static PackedScene CPS => field ??= GD.Load<PackedScene>("res://addons/idle_framework/ui_scenes/control/popup_tabs/recipe_selector/popup_tab_recipe_selector.tscn");
	
	public TextureRect NRecipeIcon;
	public Label NRecipeName;
	public Label NRecipeDescription;
	public Button NSelectButton;
	public Button NCloseButton;
	public VBoxContainer NRecipeButtonsContainer;
	public Dictionary<string, Button> NRecipeButtons = [];

	/// <summary>
	/// 对对应工厂GUID的保存，用于读写数据。需要由创建本类实例的对象负责赋值，否则实例将无法正常运作
	/// </summary>
	public Guid factoryGuidCache = Guid.Empty;

	/// <summary>
	/// 当前已选中的配方ID
	/// </summary>
	public string selectedRecipeId;
	
	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationSceneInstantiated:
				NRecipeIcon = GetNode<TextureRect>("HSC/MC/VBC/HBC/RecipeIcon");
				NRecipeName = GetNode<Label>("HSC/MC/VBC/HBC/RecipeName");
				NRecipeDescription = GetNode<Label>("HSC/MC/VBC/SC/RecipeDescription");
				NRecipeButtonsContainer = GetNode<VBoxContainer>("HSC/SC/RecipeButtonsContainer");
				NSelectButton = GetNode<Button>("HSC/MC/VBC/BottonBar/SelectButton");
				NSelectButton.Text = Localization.Tr("ui_scene_control.select_recipe");
				NCloseButton = GetNode<Button>("HSC/MC/VBC/BottonBar/CloseButton");
				NCloseButton.Text = Localization.Tr("ui_scene_control.close");
				NCloseButton.Connect(BaseButton.SignalName.Pressed, Callable.From(OnCloseButtonPressed));
				break;
		}
	}

	/// <summary>
	/// 为本实例设置内容。
	/// </summary>
	/// <param name="factoryGuid">本实例对应的工厂GUID。</param>
	public void SetContentForFactory(Guid factoryGuid)
	{
		factoryGuidCache = factoryGuid;
		if (!SaveAccess.LoadedDataHelper.QueryItemIdForGuid(factoryGuid, out string factoryItemId))
		{
			Logger.LogError(string.Format(Localization.Tr("log.error.ui_scene_control_popup_tab_recipe_selector.failed_to_query_item_id_for_factory_guid"), factoryGuid.ToString()));
			return;
		}
		if (!SaveAccess.LoadedDataHelper.UsingGameResource.FactoryRegistry.TryGetValue(factoryItemId, out FactoryRegistryObject factoryRegistryObject))
		{
			Logger.LogError(string.Format(Localization.Tr("log.error.ui_scene_control_popup_tab_recipe_selector.failed_to_get_factory_registry_object_for_factory_id"), factoryItemId));
			return;
		}
		if (factoryRegistryObject.RecipeOrder is not RecipeOrderManualSelect recipeOrderManualSelect)
		{
			Logger.LogError(string.Format(Localization.Tr("log.error.ui_scene_control_popup_tab_recipe_selector.the_recipe_order_of_target_factory_registry_object_is_not_recipe_order_manual_select"), factoryItemId));
			return;
		}
		foreach (string recipeId in recipeOrderManualSelect.RecipeIDs)
		{
			Button recipeButton = new();
			string buttonRecipeId = recipeId;
			recipeButton.Connect(BaseButton.SignalName.Pressed, Callable.From(() => OnRecipeButtonPressed(buttonRecipeId)));
			if (SaveAccess.LoadedDataHelper.UsingGameResource.RecipeRegistry.TryGetValue(recipeId, out RecipeRegistryObject recipeRegistryObject))
			{
				recipeButton.Icon = recipeRegistryObject.IconTexture;
				recipeButton.Text = Localization.Tr(recipeRegistryObject.NameKey);
			}
			NRecipeButtonsContainer.AddChild(recipeButton);
			NRecipeButtons[recipeId] = recipeButton;
		}
	}
	
	public override string GetTitleName()
	{
		return string.Format(Localization.Tr("ui_scene_control.popup_tab_title.recipe_selector"), SaveAccess.LoadedDataHelper.GetNameForInstance(factoryGuidCache, "ui_scene_control.lost_instance"));
	}

	public void OnRecipeButtonPressed(string recipeId)
	{
		selectedRecipeId = recipeId;
		if (!SaveAccess.LoadedDataHelper.UsingGameResource.RecipeRegistry.TryGetValue(recipeId, out RecipeRegistryObject recipeRegistryObject)) return;
		NRecipeIcon.Texture = recipeRegistryObject.IconTexture;
		NRecipeName.Text = Localization.Tr(recipeRegistryObject.NameKey);
		NRecipeDescription.Text = Localization.Tr(recipeRegistryObject.LoreKey);
	}

	public void OnSelectButtonPressed()
	{
		SaveAccess.LoadedDataHelper.TryEnqueueRecipeForManualFactory(factoryGuidCache, selectedRecipeId, true);
		OnCloseButtonPressed();
	}
	
	public void OnCloseButtonPressed()
	{
		EmitSignal(PopupTabBase.SignalName.Close);
	}
}
#endif
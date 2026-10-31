#if IDLE_FRAMEWORK_UISCENE_ALL || IDLE_FRAMEWORK_UISCENE_CONTROL
using System;
using System.Collections.Generic;
using System.Linq;
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

	public TabContainer NMainTC;
	public TextureRect NRecipeIcon;
	public Label NRecipeName;
	public Label NRecipeDescription;
	public TabBar NDetailTabBar;
	public VBoxContainer NDetailItemsContainer;
	public readonly List<ItemIconNameCount> NDetailItems = [];
	public Button NAddButton;
	public Button NCloseButtonInSelectionPage;
	public VBoxContainer NRecipeButtonsContainer;
	public readonly Dictionary<string, Button> NRecipeButtons = [];
	public Label NCurrentProcessingText;
	public Button NCloseButtonInQueuePage;
	public Button NCancelAllButtonInQueuePage;

	/// <summary>
	/// 对对应工厂GUID的保存，用于读写数据。需要由创建本类实例的对象负责赋值，否则实例将无法正常运作
	/// </summary>
	public Guid factoryGuidCache = Guid.Empty;

	/// <summary>
	/// 当前已选中的配方ID
	/// </summary>
	public string selectedRecipeId;

	/// <summary>
	/// 缓存当前选中的配方的注册表项。
	/// </summary>
	public RecipeRegistryObject recipeRegistryObjectCache;
	
	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationSceneInstantiated:
				NMainTC = GetNode<TabContainer>("MC/TC");
				NMainTC.SetTabTitle(0, Localization.Tr("ui_scene_control.popup_tab_recipe_selector.tab_title.select_recipe"));
				NMainTC.SetTabTitle(1, Localization.Tr("ui_scene_control.popup_tab_recipe_selector.tab_title.queue"));
				NRecipeIcon = GetNode<TextureRect>("MC/TC/SelectRecipe/MC/VBC/HBC/RecipeIcon");
				NRecipeName = GetNode<Label>("MC/TC/SelectRecipe/MC/VBC/HBC/RecipeName");
				NRecipeDescription = GetNode<Label>("MC/TC/SelectRecipe/MC/VBC/Description/RecipeDescription");
				NRecipeButtonsContainer = GetNode<VBoxContainer>("MC/TC/SelectRecipe//SC/RecipeButtonsContainer");
				NDetailTabBar = GetNode<TabBar>("MC/TC/SelectRecipe//MC/VBC/RecipeDetail/TabBar");
				NDetailTabBar.SetTabTitle(0, Localization.Tr("ui_scene_control.popup_tab_recipe_selector.ingredients"));
				NDetailTabBar.SetTabTitle(1, Localization.Tr("ui_scene_control.popup_tab_recipe_selector.results"));
				NDetailTabBar.Connect(TabBar.SignalName.TabChanged, Callable.From<int>(OnDetailTabBarTabChanged));
				NDetailItemsContainer = GetNode<VBoxContainer>("MC/TC/SelectRecipe//MC/VBC/RecipeDetail/SC/VBC");
				NAddButton = GetNode<Button>("MC/TC/SelectRecipe/MC/VBC/BottonBar/AddButton");
				NAddButton.Text = Localization.Tr("ui_scene_control.popup_tab_recipe_selector.add_to_queue");
				NAddButton.Connect(BaseButton.SignalName.Pressed, Callable.From(OnAddToQueueButtonPressed));
				NCloseButtonInSelectionPage = GetNode<Button>("MC/TC/SelectRecipe//MC/VBC/BottonBar/CloseButton");
				NCloseButtonInSelectionPage.Text = Localization.Tr("ui_scene_control.close");
				NCloseButtonInSelectionPage.Connect(BaseButton.SignalName.Pressed, Callable.From(OnCloseButtonPressed));
				NCloseButtonInQueuePage = GetNode<Button>("MC/TC/Queue/BottonBar/CloseButton");
				NCloseButtonInQueuePage.Text = Localization.Tr("ui_scene_control.close");
				NCloseButtonInQueuePage.Connect(BaseButton.SignalName.Pressed, Callable.From(OnCloseButtonPressed));
				NCurrentProcessingText = GetNode<Label>("MC/TC/Queue/CurrentText");
				NCurrentProcessingText.Text = Localization.Tr("ui_scene_control.popup_tab_recipe_selector.current_processing");
				NCancelAllButtonInQueuePage = GetNode<Button>("MC/TC/Queue/HBC/CancelAllButton");
				NCancelAllButtonInQueuePage.Connect(BaseButton.SignalName.Pressed, Callable.From(OnCancelAllButtonPressed));
				NCancelAllButtonInQueuePage.Text = Localization.Tr("ui_scene_control.popup_tab_recipe_selector.cancel_all");
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
			recipeButton.ExpandIcon = true;
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
		foreach (string recipeId in NRecipeButtons.Keys)
		{
			OnRecipeButtonPressed(recipeId);
			break;
		}
	}
	
	public override string GetTitleName()
	{
		return string.Format(Localization.Tr("ui_scene_control.popup_tab_title.recipe_selector"), SaveAccess.LoadedDataHelper.GetNameForInstance(factoryGuidCache, "ui_scene_control.lost_instance"));
	}

	public override void Update(SaveDataHelper saveDataHelper)
	{
		
	}

	/// <summary>
	/// 将配方详细信息显示为原料。
	/// </summary>
	/// <param name="recipeRegistryObject">要切换到的配方的注册表项。</param>
	public void SetDetailIngredients(RecipeRegistryObject recipeRegistryObject)
	{
		// 创建物品对象达到原料总数
		while (NDetailItems.Count < recipeRegistryObject.Ingredients.Count)
		{
			ItemIconNameCount newDetailItem = ItemIconNameCount.Create();
			NDetailItems.Add(newDetailItem);
			NDetailItemsContainer.AddChild(newDetailItem);
		}
		// 遍历原料
		int index = 0;
		foreach ((string itemId, NumberProvider countNumberProvider) in recipeRegistryObject.Ingredients
			.OrderByDescending(ingredient => ingredient.Value)
			.ThenBy(ingredient => ingredient.Key))
		{
			ItemIconNameCount detailItem = NDetailItems[index];
			detailItem.Visible = true;
			detailItem.itemId = itemId;
			if (SaveAccess.LoadedDataHelper.UsingGameResource.ItemRegistry.TryGetValue(itemId, out ItemRegistryObject itemRegistryObject))
			{
				// 能找到物品
				detailItem.NItemButton.Text = Localization.Tr(itemRegistryObject.NameKey);
				detailItem.NItemButton.Icon = itemRegistryObject.IconTexture;
			}
			else
			{
				// 不能找到物品
				detailItem.NItemButton.Text = itemId;
				detailItem.NItemButton.Icon = new PlaceholderTexture2D();
			}
			detailItem.NCount.Text = countNumberProvider.NumberPreviewText;
			index++;
		}
		// 遍历后续的多余物品对象，使它们不可见
		for (; index < NDetailItems.Count; index++) NDetailItems[index].Visible = false;
	}

	/// <summary>
	/// 将配方详细信息显示为产物。
	/// </summary>
	/// <param name="recipeRegistryObject">要切换到的配方的注册表项。</param>
	public void SetDetailResults(RecipeRegistryObject recipeRegistryObject)
	{
		// 创建物品对象达到产物总数
		while (NDetailItems.Count < recipeRegistryObject.Results.Count)
		{
			ItemIconNameCount newDetailItem = ItemIconNameCount.Create();
			NDetailItems.Add(newDetailItem);
			NDetailItemsContainer.AddChild(newDetailItem);
		}
		// 遍历产物
		int index = 0;
		foreach ((string itemId, NumberProvider countNumberProvider) in recipeRegistryObject.Results
			.OrderByDescending(result => result.Value)
			.ThenBy(result => result.Key))
		{
			ItemIconNameCount detailItem = NDetailItems[index];
			detailItem.Visible = true;
			detailItem.itemId = itemId;
			if (SaveAccess.LoadedDataHelper.UsingGameResource.ItemRegistry.TryGetValue(itemId, out ItemRegistryObject itemRegistryObject))
			{
				// 能找到物品
				detailItem.NItemButton.Text = Localization.Tr(itemRegistryObject.NameKey);
				detailItem.NItemButton.Icon = itemRegistryObject.IconTexture;
			}
			else
			{
				// 不能找到物品
				detailItem.NItemButton.Text = itemId;
				detailItem.NItemButton.Icon = new PlaceholderTexture2D();
			}
			detailItem.NCount.Text = countNumberProvider.NumberPreviewText;
			index++;
		}
		// 遍历后续的多余物品对象，使它们不可见
		for (; index < NDetailItems.Count; index++) NDetailItems[index].Visible = false;
	}

	public void OnDetailTabBarTabChanged(int index)
	{
		switch (index)
		{
			case 0:
				SetDetailIngredients(recipeRegistryObjectCache);
				break;
			case 1:
				SetDetailResults(recipeRegistryObjectCache);
				break;
		}
	}
	
	public void OnRecipeButtonPressed(string recipeId)
	{
		selectedRecipeId = recipeId;
		if (!SaveAccess.LoadedDataHelper.UsingGameResource.RecipeRegistry.TryGetValue(recipeId, out RecipeRegistryObject recipeRegistryObject)) return;
		NRecipeIcon.Texture = recipeRegistryObject.IconTexture;
		NRecipeName.Text = Localization.Tr(recipeRegistryObject.NameKey);
		NRecipeDescription.Text = Localization.Tr(recipeRegistryObject.LoreKey);
		recipeRegistryObjectCache = recipeRegistryObject;
		OnDetailTabBarTabChanged(NDetailTabBar.CurrentTab);
	}

	public void OnAddToQueueButtonPressed()
	{
		SaveAccess.LoadedDataHelper.TryEnqueueRecipeForManualFactory(factoryGuidCache, selectedRecipeId, true);
	}

	public void OnCancelAllButtonPressed()
	{
		
	}
	
	public void OnCloseButtonPressed()
	{
		EmitSignal(PopupTabBase.SignalName.Close);
	}
}
#endif
#if IDLE_FRAMEWORK_UISCENE_ALL || IDLE_FRAMEWORK_UISCENE_CONTROL
using Godot;
using IdleFramework.Global;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// [IdleFramework内置UI场景-控件主题]配方选择弹窗标签页中显示的原料或产物详情的单个对象。
/// </summary>
[GlobalClass]
public partial class ItemIconNameCount : HBoxContainer, IClassPackedScene
{
	public static PackedScene CPS => field ??= GD.Load<PackedScene>("res://addons/idle_framework/ui_scenes/control/popup_tabs/recipe_selector/item_icon_name_count.tscn");
	
	public Button NItemButton;
	public Label NCount;

	/// <summary>
	/// 该对象所代表的物品ID，用于当被点击时创建物品详细信息弹窗。
	/// </summary>
	public string itemId;

	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationSceneInstantiated:
				NItemButton = GetNode<Button>("ItemButton");
				NItemButton.Connect(BaseButton.SignalName.Pressed, Callable.From(OnButtonPressed));
				NCount = GetNode<Label>("Count");
				break;
		}
	}

	public void OnButtonPressed()
	{
		Control.TabPopupInstance.TryOpenTabOrPullToAddIfUnexist("ItemDetail." + itemId, () =>
		{
			PopupTabItemDetail popupTabItemDetail = PopupTabItemDetail.Create();
			popupTabItemDetail.SetContentForItemId(itemId);
			return popupTabItemDetail;
		});
	}
}
#endif
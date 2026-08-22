#if IDLE_FRAMEWORK_UISCENE_ALL || IDLE_FRAMEWORK_UISCENE_CONTROL
using Godot;
using IdleFramework.Core;
using IdleFramework.Global;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// [IdleFramework内置UI场景-控件主题]容器实例展开式容器，在空间详细区域容器中显示一个容器实例的内容物
/// </summary>
[GlobalClass]
public partial class PopupTabItemDetail : PopupTabBase, IClassPackedScene
{
	public static PackedScene CPS => field ??= GD.Load<PackedScene>("res://addons/idle_framework/ui_scenes/control/popup_tabs/item_detail/popup_tab_item_detail.tscn");

	public TextureRect NItemIcon;
	public Label NItemName;
	public Label NItemDescription;
	public Button NPinButton;
	public Button NCloseButton;

	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationSceneInstantiated:
				NItemIcon = GetNode<TextureRect>("MC/VBC/HBC/ItemIcon");
				NItemName = GetNode<Label>("MC/VBC/HBC/ItemName");
				NItemDescription = GetNode<Label>("MC/VBC/SC/ItemDescription");
				NPinButton = GetNode<Button>("MC/VBC/BottonBar/PinButton");
				NPinButton.Text = Localization.Tr("ui_scene_control.pin");
				NCloseButton = GetNode<Button>("MC/VBC/BottonBar/CloseButton");
				NCloseButton.Text = Localization.Tr("ui_scene_control.close");
				NCloseButton.Connect(BaseButton.SignalName.Pressed, Callable.From(OnCloseButtonPressed));
				break;
		}
	}

	public void SetContentForItemId(string itemId)
	{
		if (!SaveAccess.LoadedDataHelper.UsingGameResource.ItemRegistry.TryGetValue(itemId, out ItemRegistryObject itemRegistryObject)) return;
		NItemIcon.Texture = itemRegistryObject.IconTexture;
		NItemName.Text = Localization.Tr(itemRegistryObject.NameKey);
		NItemDescription.Text = Localization.Tr(itemRegistryObject.LoreKey);
	}

	public override string GetTitleName()
	{
		return string.Format(Localization.Tr("ui_scene_control.popup_tab_title.item_detail"), NItemName.Text);
	}

	public void OnCloseButtonPressed()
	{
		EmitSignal(PopupTabBase.SignalName.Close);
	}
}
#endif
#if IDLE_FRAMEWORK_UISCENE_ALL || IDLE_FRAMEWORK_UISCENE_CONTROL
using Godot;
using IdleFramework.Global;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// [IdleFramework内置UI场景-控件主题]配方选择弹窗标签页中显示的单个配方队列项目。
/// </summary>
[GlobalClass]
public partial class RecipeQueueObject : HBoxContainer, IClassPackedScene
{
	public static PackedScene CPS => field ??= GD.Load<PackedScene>("res://addons/idle_framework/ui_scenes/control/popup_tabs/recipe_selector/recipe_queue_object.tscn");

	public TextureRect NIcon;
	public Button NCancelButton;
	public Label NName;

	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationSceneInstantiated:
				NIcon = GetNode<TextureRect>("Icon");
				NCancelButton = GetNode<Button>("CancelButton");
				NCancelButton.Connect(BaseButton.SignalName.Pressed, Callable.From(OnButtonPressed));
				NCancelButton.Text = Localization.Tr("ui_scene_control.recipe_queue_object.cancel");
				NName = GetNode<Label>("Name");
				break;
		}
	}

	public void OnButtonPressed()
	{
		
	}
}
#endif
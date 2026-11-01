#if IDLE_FRAMEWORK_UISCENE_ALL || IDLE_FRAMEWORK_UISCENE_CONTROL
using System.Diagnostics.CodeAnalysis;
using Godot;
using IdleFramework.Core;
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

	public string recipeId;

	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationSceneInstantiated:
				NIcon = GetNode<TextureRect>("Icon");
				NCancelButton = GetNode<Button>("CancelButton");
				NCancelButton.Connect(BaseButton.SignalName.Pressed, Callable.From(OnCancelButtonPressed));
				NCancelButton.Text = Localization.Tr("ui_scene_control.recipe_queue_object.cancel");
				NName = GetNode<Label>("Name");
				break;
		}
	}

	/// <summary>
	/// 设置该配方队列项目的配方。留空时清除。如果给定的ID与原本相同则不会更新。
	/// </summary>
	/// <param name="id">该配方的ID，为空或<c>null</c>时进行清除。</param>
	/// <param name="recipeRegistryObject">该配方的注册表项，执行清除时可以为<c>null</c>，否则若为<c>null</c>将保持图标、名称空白。</param>
	public void SetRecipe([AllowNull]string id, [AllowNull]RecipeRegistryObject recipeRegistryObject)
	{
		if (id == recipeId) return;
		if (string.IsNullOrEmpty(id))
		{ 
			recipeId = string.Empty;
			NIcon.Texture = null;
			NName.Text = "--";
			NCancelButton.Disabled = true;
			return;
		}
		recipeId = id;
		if (recipeRegistryObject == null)
		{
			NIcon.Texture = null;
			NName.Text = "--";
		}
		else
		{
			NIcon.Texture = recipeRegistryObject.IconTexture;
			NName.Text = Localization.Tr(recipeRegistryObject.NameKey);
		}
		NCancelButton.Disabled = false;
	}

	/// <summary>
	/// 将该配方队列项目设置为丢失配方注册表项的样子，同时清空它的ID。
	/// </summary>
	public void SetRecipeLost()
	{
		recipeId = string.Empty;
		NIcon.Texture = new PlaceholderTexture2D();
		NName.Text = "??";
		NCancelButton.Disabled = false;
	}

	public void OnCancelButtonPressed()
	{
		if (string.IsNullOrEmpty(recipeId)) return;
		// TODO
	}
}
#endif
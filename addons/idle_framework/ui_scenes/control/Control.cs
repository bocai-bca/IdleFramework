#if IDLE_FRAMEWORK_UISCENE_CONTROL
using System.Collections.Generic;
using Godot;
using IdleFramework.Core;
using IdleFramework.Global;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// [IdleFramework内置UI场景-控件主题]根节点
/// </summary>
[GlobalClass]
public partial class Control : UIScene, IClassPackedScene
{
	public static PackedScene CPS => field ??= GD.Load<PackedScene>("res://addons/idle_framework/ui_scenes/control/ui_scene_control.tscn");

	public override Vector2I WindowMinSize => new(960, 720);

	public override Godot.Collections.Array<Translation> UISceneTranslations
	{
		get
		{
			field ??=
			[
				GD.Load<Translation>("res://addons/idle_framework/ui_scenes/control/lang/ui_scene.en.translation"),
				GD.Load<Translation>("res://addons/idle_framework/ui_scenes/control/lang/ui_scene.zh.translation"),
			];
			return field;
		}
	}

	public TextureRect NTopBar_Icon;
	public Label NTopBar_GameTitle;
	public HBoxContainer NTopBar_PinnedItemsBar;
	public VBoxContainer NMainSpace_SpaceButtons;
	public PanelContainer NMainSpace_DetailArea;
	public readonly Dictionary<string, SpaceDetailArea> NSpaceDetailAreas = [];
	public readonly Dictionary<string, SpaceButton> NSpaceButtons = [];
	
	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationSceneInstantiated:
				NTopBar_Icon = GetNode<TextureRect>("VBC/TopBar/VBC/Icon");
				NTopBar_GameTitle = GetNode<Label>("VBC/TopBar/VBC/Name");
				NTopBar_PinnedItemsBar = GetNode<HBoxContainer>("VBC/TopBar/VBC/PinnedItemsBar");
				NMainSpace_SpaceButtons = GetNode<VBoxContainer>("VBC/MainSpace/SideBar/SC/SpaceButtons");
				NMainSpace_DetailArea = GetNode<PanelContainer>("VBC/MainSpace/DetailArea");
				break;
		}
	}

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public override void OnGameResourceReady()
	{
		GameResource gameResource = MotherNode.Instance.GameResource;
		NTopBar_GameTitle.Text = Localization.Tr(gameResource.NameKey);
		NTopBar_Icon.Texture = gameResource.IconTexture;
		foreach ((string spaceId, SpaceRegistryObject spaceRegistryObject) in gameResource.SpaceRegistry)
		{
			SpaceButton spaceButton = SpaceButton.Create();
			spaceButton.Text = Localization.Tr(spaceRegistryObject.NameKey);
			//spaceButton.SpaceID = spaceId;
			spaceButton.Icon = spaceRegistryObject.IconTexture;
			spaceButton.Pressed += () => OnSpaceButtonPressed(spaceId);
			NMainSpace_SpaceButtons.AddChild(spaceButton);
			NSpaceButtons[spaceId] = spaceButton;
			SpaceDetailArea newSpaceDetailArea = SpaceDetailArea.Create();
			newSpaceDetailArea.Name = spaceId;
			newSpaceDetailArea.SpaceId = spaceId;
			NSpaceDetailAreas[spaceId] = newSpaceDetailArea;
			NMainSpace_DetailArea.AddChild(newSpaceDetailArea);
		}
	}

	public override void OnUpdaterDone(SaveDataHelper saveDataHelper)
	{
		foreach ((string spaceId, SpaceDetailArea spaceDetailArea) in NSpaceDetailAreas)
		{
			spaceDetailArea.Update(saveDataHelper);
		}
	}

	public void OnSpaceButtonPressed(string pressedSpaceId)
	{
		foreach ((string detailAreaSpaceId, SpaceDetailArea spaceDetailArea) in NSpaceDetailAreas)
		{
			spaceDetailArea.Visible = detailAreaSpaceId == pressedSpaceId;
		}
	}
}
#endif
using Godot;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// [IdleFramework内置UI场景-控件主题]的
/// </summary>
[GlobalClass]
public partial class PinnedItem : PanelContainer
{
	public TextureRect NIcon;

	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationSceneInstantiated:
				NIcon = GetNode<TextureRect>("MC/HBC/Icon");
				break;
		}
	}
}

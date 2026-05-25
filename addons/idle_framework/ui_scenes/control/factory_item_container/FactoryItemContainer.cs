#if IDLE_FRAMEWORK_UISCENE_CONTROL
using System;
using Godot;
using IdleFramework.Global;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// [IdleFramework内置UI场景-控件主题]工厂实例展开式容器，在空间详细区域容器中显示一个工厂实例
/// </summary>
[GlobalClass]
public partial class FactoryItemContainer : Godot.Control, IClassPackedScene
{
	public static PackedScene CPS => field ??= GD.Load<PackedScene>("res://addons/idle_framework/ui_scenes/control/factory_item_container/factory_item_container.tscn");
	
	public FoldableContainer NFC;
	
	/// <summary>
	/// 对应的工厂实例的GUID
	/// </summary>
	public Guid FactoryGuid { get; set; }
	
	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationSceneInstantiated:
				NFC = GetNode<FoldableContainer>("FC");
				break;
		}
	}
	
	/// <summary>
	/// 设置该工厂实例展开式容器的标题名，应对应其物品实例的名称
	/// </summary>
	/// <param name="titleName">要设置为的标题名。</param>
	public void SetTitleName(string titleName) => NFC.Title = titleName;
}
#endif
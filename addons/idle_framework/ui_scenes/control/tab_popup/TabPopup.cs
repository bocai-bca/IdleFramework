#if IDLE_FRAMEWORK_UISCENE_ALL || IDLE_FRAMEWORK_UISCENE_CONTROL
using Godot;
using IdleFramework.Global;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// [IdleFramework内置UI场景-控件主题]弹出窗口，单个窗口可以容纳多个标签页，用于显示需要以弹出窗口呈现的临时内容
/// </summary>
[GlobalClass]
public partial class TabPopup : TabContainer, IClassPackedScene
{
	public static PackedScene CPS => field ??= GD.Load<PackedScene>("res://addons/idle_framework/ui_scenes/control/popup/popup.tscn");

	/// <summary>
	/// 信号-当添加新弹窗标签页时放出，可用于提示父级节点该显示弹窗了
	/// </summary>
	[Signal]
	public delegate void AddedTabsEventHandler();
	
	/// <summary>
	/// 信号-当所有弹窗标签页均被关闭时放出，可用于提示父级节点该隐藏弹窗了
	/// </summary>
	[Signal]
	public delegate void TabsAllClosedEventHandler();
	
	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationSceneInstantiated:
				
				break;
		}
	}

	/// <summary>
	/// 信号方法-当标签页焦点改变。
	/// </summary>
	/// <param name="tabIndex">新的焦点标签页的索引。</param>
	public void OnTabChanged(int tabIndex)
	{
		if (GetChildCount() == 0)
		{
			EmitSignal(SignalName.TabsAllClosed);
		}
	}
}
#endif
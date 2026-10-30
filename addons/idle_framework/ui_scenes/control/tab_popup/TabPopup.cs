#if IDLE_FRAMEWORK_UISCENE_ALL || IDLE_FRAMEWORK_UISCENE_CONTROL
using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
using IdleFramework.Core;
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
	public delegate void AddedTabsEventHandler(string tabName);
	
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
			case NotificationChildOrderChanged:
				if (GetChildCount() == 0) EmitSignal(SignalName.TabsAllClosed);
				break;
			case NotificationDraw:
				if (GetParent() is PanelContainer parent) CustomMinimumSize = parent.Size * 0.75f;
				break;
		}
	}

	public void Update(SaveDataHelper saveDataHelper)
	{
		foreach (Node child in GetChildren()) if (child is PopupTabBase childPopupTab) childPopupTab.Update(saveDataHelper);
	}

	/// <summary>
	/// 尝试添加标签页，并返回是否成功。如果已有同名标签页，则会放弃添加。若成功添加会同时为节点连接关闭信号。
	/// 本方法不会打开标签页。
	/// </summary>
	/// <param name="tabName">要添加的标签页名称。</param>
	/// <param name="tabNode">要添加的标签页节点。</param>
	/// <returns>是否成功添加。如果已有同名标签页，则会添加失败。</returns>
	public bool TryAddTab(string tabName, PopupTabBase tabNode)
	{
		if (TryGetTab(tabName, out _)) return false;
		tabNode.TabName = tabName;
		AddChild(tabNode);
		int index = GetChildCount() - 1;
		GetTabBar().SetTabTitle(index, tabNode.GetTitleName());
		tabNode.Connect(PopupTabBase.SignalName.Close, Callable.From(() => CloseTab(tabNode)));
		EmitSignal(SignalName.AddedTabs, tabName);
		return true;
	}

	/// <summary>
	/// 尝试添加标签页并打开，并返回是否成功添加。如果已有同名标签页，将放弃添加并直接打开对应标签页，届时也会返回<c>false</c>。若成功添加会同时为节点连接关闭信号。
	/// </summary>
	/// <param name="tabName">要添加的标签页名称。</param>
	/// <param name="tabNode">要添加的标签页节点。</param>
	/// <returns>是否成功添加。如果已有同名标签页，则会添加失败。即使成功打开已存在的同名标签页，本返回值仍为<c>false</c>。</returns>
	[Obsolete("TryOpenTabOrPullToAddIfUnexist() is better.")]
	public bool TryAddTabAndOpen(string tabName, PopupTabBase tabNode)
	{
		if (TryOpenTab(tabName)) return false;
		tabNode.TabName = tabName;
		AddChild(tabNode);
		int index = GetChildCount() - 1;
		GetTabBar().SetTabTitle(index, tabNode.GetTitleName());
		CurrentTab = index;
		tabNode.Connect(PopupTabBase.SignalName.Close, Callable.From(() => CloseTab(tabNode)));
		EmitSignal(SignalName.AddedTabs, tabName);
		return true;
	}

	/// <summary>
	/// 尝试打开标签页，并返回是否成功打开。如果没有找到同名标签页，将从给定的标签页创建函数获取一个标签页实例，届时也会返回<c>false</c>。若成功添加会同时为节点连接关闭信号。
	/// 相比于<c>TryAddTabAndOpen()</c>，可以在如果已有同名标签页的情况下避免重复创建无用标签页而节省性能。
	/// </summary>
	/// <param name="tabName">要打开的标签页名称。</param>
	/// <param name="tabNodeCreator">标签页创建函数，如果未找到已有标签页则会调用本函数，如果本函数提供<c>null</c>则将放弃添加新标签页。</param>
	/// <returns>是否成功找到并返回给定名称的标签页，若一开始没有找到，即使后续通过创建函数创建新标签页并添加再打开，也仍然返回<c>false</c>。</returns>
	public bool TryOpenTabOrPullToAddIfUnexist(string tabName, [DisallowNull]Func<PopupTabBase> tabNodeCreator)
	{
		if (TryOpenTab(tabName)) return true;
		PopupTabBase newTabNode = tabNodeCreator();
		if (newTabNode is null) return false;
		newTabNode.TabName = tabName;
		AddChild(newTabNode);
		int index = GetChildCount() - 1;
		GetTabBar().SetTabTitle(index, newTabNode.GetTitleName());
		CurrentTab = index;
		newTabNode.Connect(PopupTabBase.SignalName.Close, Callable.From(() => CloseTab(newTabNode)));
		EmitSignal(SignalName.AddedTabs, tabName);
		return false;
	}
	
	/// <summary>
	/// 尝试获取拥有特定名称的标签页的节点，并返回是否成功。
	/// </summary>
	/// <param name="tabName">要获取节点的标签页的名称。</param>
	/// <param name="tabNode">对应标签页的节点，如果没有成功获取到则为<c>null</c>。</param>
	/// <returns>是否成功获取指定标签页的节点，如果当前没有对应名称的标签页，则返回<c>false</c>。</returns>
	public bool TryGetTab(string tabName, out PopupTabBase tabNode)
	{
		foreach (Node child in GetChildren())
		{
			if (child is not PopupTabBase childPopupTab) continue;
			if (childPopupTab.TabName != tabName) continue;
			tabNode = childPopupTab;
			return true;
		}
		tabNode = null;
		return false;
	}
	
	/// <summary>
	/// 尝试移除拥有特定名称的标签页，并返回是否成功。
	/// </summary>
	/// <param name="tabName">要移除的标签页的名称。</param>
	/// <returns>是否成功移除指定标签页，如果当前没有对应名称的标签页，则返回<c>false</c>。</returns>
	public bool TryRemoveTab(string tabName)
	{
		if (!TryGetTab(tabName, out PopupTabBase tabNode)) return false;
		CloseTab(tabNode);
		return true;
	}

	/// <summary>
	/// 尝试将焦点移交到拥有特定名称的标签页，并返回是否成功。
	/// </summary>
	/// <param name="tabName">要聚焦的标签页的名称。</param>
	/// <returns>是否成功聚焦到指定标签页，如果当前没有对应名称的标签页，则返回<c>false</c>。</returns>
	public bool TryOpenTab(string tabName)
	{
		int childCount = GetChildCount();
		for (int i = 0; i < childCount; i++)
		{
			PopupTabBase tabNode = GetChildOrNull<PopupTabBase>(i);
			if (tabNode is null || tabNode.TabName != tabName) continue;
			CurrentTab = i;
			return true;
		}
		return false;
	}

	/// <summary>
	/// 直接关闭一个标签页。将释放该节点并做子节点数量判断，可能发出<c>TabsAllClosed</c>信号。
	/// </summary>
	/// <param name="tabNode">要关闭的标签页节点。</param>
	private static void CloseTab(PopupTabBase tabNode)
	{
		tabNode.QueueFree();
	}
}
#endif
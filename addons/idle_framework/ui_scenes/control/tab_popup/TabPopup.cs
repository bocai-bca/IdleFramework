#if IDLE_FRAMEWORK_UISCENE_ALL || IDLE_FRAMEWORK_UISCENE_CONTROL
using System;
using System.Collections.Generic;
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

	/// <summary>
	/// 当前的所有标签页表，键为标签页的易识名(用于避免重复添加相同内容的标签页)，值为对对应标签页节点的索引和节点索引。
	/// </summary>
	public Dictionary<string, KeyValuePair<int, PopupTabBase>> tabs = [];
	
	//TODO 由于PopupTabBase的框架设计颠覆，本类现有的方法和tabs字段也要重构，具体怎么重构还没想好，自己看着办吧

	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationSceneInstantiated:
				break;
		}
	}

	/// <summary>
	/// 尝试添加标签页，并返回是否成功。如果已有同名标签页，则会放弃添加。
	/// </summary>
	/// <param name="tabName">要添加的标签页名称。</param>
	/// <param name="tabNode">要添加的标签页节点。</param>
	/// <returns>是否成功添加。如果已有同名标签页，则会添加失败。</returns>
	public bool TryAddTab(string tabName, PopupTabBase tabNode)
	{
		if (tabs.ContainsKey(tabName)) return false;
		tabNode.TabName = tabName;
		AddChild(tabNode);
		tabs[tabName] = new KeyValuePair<int, PopupTabBase>(GetTabCount() - 1, tabNode);
		EmitSignal(SignalName.AddedTabs);
		return true;
	}
	
	/// <summary>
	/// 尝试获取拥有特定名称的标签页的节点，并返回是否成功。
	/// </summary>
	/// <param name="tabName">要获取节点的标签页的名称。</param>
	/// <param name="tabNode">对应标签页的节点，如果没有成功获取到则为<c>null</c>。</param>
	/// <returns>是否成功获取指定标签页的节点，如果当前没有对应名称的标签页，则返回<c>false</c>。</returns>
	public bool TryGetTab(string tabName, out PopupTabBase tabNode)
	{
		if (!tabs.TryGetValue(tabName, out KeyValuePair<int, PopupTabBase> tab))
		{
			tabNode = null;
			return false;
		}
		tabNode = tab.Value;
		return true;
	}
	
	/// <summary>
	/// 尝试移除拥有特定名称的标签页，并返回是否成功。
	/// </summary>
	/// <param name="tabName">要移除的标签页的名称。</param>
	/// <returns>是否成功移除指定标签页，如果当前没有对应名称的标签页，则返回<c>false</c>。</returns>
	public bool TryRemoveTab(string tabName)
	{
		if (!tabs.TryGetValue(tabName, out KeyValuePair<int, PopupTabBase> tabTarget)) return false;
		foreach ((string tabNameInList, KeyValuePair<int, PopupTabBase> tabInAll) in tabs)
		{
			if (tabInAll.Key <= tabTarget.Key) continue;
			tabs[tabNameInList] = new KeyValuePair<int, PopupTabBase>(tabs[tabNameInList].Key - 1, tabs[tabNameInList].Value);
		}
		tabTarget.Value.QueueFree();
		tabs.Remove(tabName);
		if (tabs.Count == 0) EmitSignal(SignalName.TabsAllClosed);
		return true;
	}

	/// <summary>
	/// 尝试将焦点移交到拥有特定名称的标签页，并返回是否成功。
	/// </summary>
	/// <param name="tabName">要聚焦的标签页的名称。</param>
	/// <returns>是否成功聚焦到指定标签页，如果当前没有对应名称的标签页，则返回<c>false</c>。</returns>
	public bool TryOpenTab(string tabName)
	{
		if (!tabs.TryGetValue(tabName, out KeyValuePair<int, PopupTabBase> tabTarget)) return false;
		CurrentTab = tabTarget.Key;
		return true;
	}
}
#endif
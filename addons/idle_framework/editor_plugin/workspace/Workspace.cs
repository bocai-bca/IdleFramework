using Godot;
using System.Collections.Generic;

namespace IdleFramework.EditorPlugin;

/// <summary>
/// 放置引擎编辑器插件的工作区
/// </summary>
public partial class Workspace : PanelContainer
{
	/// <summary>
	/// 节点-游戏资源索引
	/// </summary>
	public TabContainer NGameResourceIndex { get; private set; }
	
	/// <summary>
	/// 工作区节点树中游戏资源索引容器的标签页枚举
	/// </summary>
	public enum GameResourceIndexTabs
	{
		ItemRegistry = 0,
	}

	/// <summary>
	/// 工作区节点树中游戏资源索引容器的标签页索引表(对应相应标签页在节点HBC/GameResourceIndex中的排序)
	/// </summary>
	public static readonly Dictionary<GameResourceIndexTabs, int> GameResourceTabsIndex = new()
	{
		{ GameResourceIndexTabs.ItemRegistry, 0 },
	};
	
	public override void _Ready()
	{
		NGameResourceIndex = GetNode<TabContainer>("HBC/GameResourceIndex");
		NGameResourceIndex.SetTabTitle(GameResourceTabsIndex[GameResourceIndexTabs.ItemRegistry], Localization.Tr("item_registry"));
	}
}

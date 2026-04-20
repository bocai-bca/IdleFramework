using System;
using Godot;
using Godot.Collections;

namespace IdleFramework;

/// <summary>
/// 物品最大堆叠数提供器
/// </summary>
[GlobalClass]
public partial class ItemMaxStacksProvider : Resource
{
	/// <summary>
	/// 提供器模式枚举
	/// </summary>
	public enum ProviderMode
	{
		/// <summary>
		/// 覆写模式，对数据列表中含有的物品的最大堆叠数量进行覆写，不含有的物品遵照其原本的默认值
		/// </summary>
		Overriding = 0,
		/// <summary>
		/// 过滤模式，对数据列表中含有的物品的最大堆叠数量进行覆写，不含有的物品返回0
		/// </summary>
		Filting = 1,
	}
	
	/// <summary>
	/// 提供器模式
	/// </summary>
	[Export]
	[ExportGroup("Data")]
	public ProviderMode Mode { get; set; } = ProviderMode.Overriding;

	/// <summary>
	/// 提供器的数据列表
	/// </summary>
	[Export]
	public Dictionary<StringName, int> List = new();

	/// <summary>
	/// 从本提供器中根据给定物品的ID获取其堆叠数量。如果给定的物品注册表中没有注册该物品，则无论本提供器的模式如何，都将返回0
	/// </summary>
	/// <param name="itemRegistry">物品注册表</param>
	/// <param name="itemId">需要获取最大堆叠数量的物品ID</param>
	/// <exception cref="ArgumentOutOfRangeException">本提供器的Mode是无效值时抛出</exception>
	/// <returns>该物品的最大堆叠数量</returns>
	public int GetCountForItem(Dictionary<StringName, ItemRegistryObject> itemRegistry, StringName itemId)
	{
		if (!itemRegistry.TryGetValue(itemId, out ItemRegistryObject itemRegistryObject)) return 0;
		if (List.TryGetValue(itemId, out int overrideStackCount))
		{
			return overrideStackCount;
		}
		return Mode switch
		{
			ProviderMode.Overriding => itemRegistryObject.DefaultMaxStackCount,
			ProviderMode.Filting => 0,
			_ => throw new ArgumentOutOfRangeException(),
		};
	}

	/// <summary>
	/// 从本提供器中根据给定物品的ID获取其堆叠数量。将自动从主节点获取游戏资源，如果过程中失败将返回0，如果获取到的物品注册表中没有注册该物品，则无论本提供器的模式如何，都将返回0
	/// 性能提示：
	///		如果不嫌麻烦建议主动管理物品注册表的引用然后使用双参数重载，使用本重载可能会比双参数重载多耗费一点性能。本重载是设计给使用者编写脚本更简便用的。
	/// </summary>
	/// <param name="itemId">需要获取最大堆叠数量的物品ID</param>
	/// <returns>该物品的最大堆叠数量</returns>
	public int GetCountForItem(StringName itemId)
	{
		Dictionary<StringName, ItemRegistryObject> itemRegistry = MotherNode.Instance?.GameResource?.ItemRegistry;
		return itemRegistry == null ? 0 : GetCountForItem(itemRegistry, itemId);
	}
}
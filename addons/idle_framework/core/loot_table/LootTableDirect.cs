using System.Collections.Generic;
using Godot;

namespace IdleFramework.Core;

/// <summary>
/// 掉落表-直接提供
/// 指定一个固定的掉落表，每次都返回固定的物品，数量可变
/// </summary>
[GlobalClass]
public partial class LootTableDirect: LootTable
{
	public override Dictionary<string, long> GetLoot()
	{
		Dictionary<string, long> result = [];
		foreach ((string itemId, NumberProvider numberProvider) in Items)
		{
			result.TryAdd(itemId, 0L);
			result[itemId] += numberProvider.GetNumber();
		}
		return result;
	}
	
	/// <summary>
	/// 物品表，键代表物品ID，值代表数量
	/// </summary>
	[Export]
	[ExportGroup("Data")]
	public Godot.Collections.Dictionary<string, NumberProvider> Items { get; set; } = [];

}
using System;
using System.Collections.Generic;
using Godot;

namespace IdleFramework.Core;

/// <summary>
/// 掉落表-列表取一
/// 指定一个固定的表，每次从中返回一个物品，数量恒为一
/// </summary>
[GlobalClass]
public partial class LootTableListPickOne: LootTable
{
	public override Dictionary<string, long> GetLoot()
	{
		Dictionary<string, long> result = [];
		if (Items.Count <= 0) return result;
		int index = Random.Shared.Next(0, Items.Count);
		result.Add(Items[index], 1L);
		return result;
	}

	/// <summary>
	/// 物品ID表。
	/// </summary>
	[Export]
	[ExportGroup("Data")]
	public Godot.Collections.Array<string> Items { get; set; } = [];

}
using System;
using System.Collections.Generic;
using Godot;
using IdleFramework.Global;

namespace IdleFramework;

/// <summary>
/// 掉落表-池抽取
/// 指定数个的掉落表，从中抽取一或多个进行执行，提供了一些控制参数。
/// </summary>
[GlobalClass]
public partial class LootTablePoolRoll: LootTable
{
	public override Dictionary<string, long> GetLoot()
	{
		if (recursionLock)
		{
			Logger.LogError(Localization.Tr("log.error.loot_table_pool_roll.unexcepted_call_blocked_by_recursion_lock"));
			return [];
		}
		recursionLock = true; // 上锁
		Dictionary<string, long> result = []; // 创建结果表，键代表物品ID，值代表数量
		try
		{
			long rollTimes = Roll?.GetNumber() ?? 0L; // 计算抽取轮数
			bool[] rejectDuplicate = new bool[PoolEntries.Count]; // 已抽到的项目的表，索引与PoolEntries一一对应
			long[] weightTable = new long[PoolEntries.Count]; // 创建用于每一轮抽取的权重表
			for (; rollTimes > 0; rollTimes--) // 遍历抽取轮数次
			{
				for (int i = 0; i < weightTable.Length; i++) // 构造权重表
				{
					LootTableRollableEntry currentPoolEntry = PoolEntries[i];
					if (currentPoolEntry is null) // 池项防null检查
					{
						Logger.LogError(string.Format(Localization.Tr("log.error.loot_table_pool_roll.a_pool_entry_is_null"), i));
						continue;
					}
					if (currentPoolEntry.Weight is null)
					{
						Logger.LogError(string.Format(Localization.Tr("log.error.loot_table_pool_roll.the_weight_of_a_pool_entry_is_null"), i));
						continue;
					}
					weightTable[i] = rejectDuplicate[i] ? 0L : Math.Clamp(currentPoolEntry.Weight.GetNumber(), 0L, long.MaxValue); // 写入权重表，如果该项已被抽取到且该项不允许重复抽取，则权重设为0
				}
				int currentRoll = weightTable.RandomWeighted(Random.Shared); // 抽
				if (!PoolEntries[currentRoll].AllowDuplicates) rejectDuplicate[currentRoll] = true; // 如果对应项不允许被重复抽，则对该项标记上
				LootTable rolledLootTable = PoolEntries[currentRoll].LootTable;
				if (rolledLootTable is null)
				{
					Logger.LogError(string.Format(Localization.Tr("log.error.loot_table_pool_roll.the_loot_table_of_a_pool_entry_is_null"), currentRoll));
					continue;
				}
				foreach ((string itemId, long itemCount) in rolledLootTable.GetLoot())
				{
					result.TryAdd(itemId, 0L);
					result[itemId] += itemCount;
				}
			}
		}
		catch (Exception e)
		{
			Logger.LogError(string.Format(Localization.Tr("log.error.loot_table_pool_roll.catched_exception_on_calling_loot_tables_in_pool"), e));
			recursionLock = false;
			return [];
		}
		recursionLock = false;
		return result;
	}
	
	/// <summary>
	/// 掉落表抽取池，将从中抽取掉落表用以执行。
	/// </summary>
	[Export]
	[ExportGroup("Data")]
	public Godot.Collections.Array<LootTableRollableEntry> PoolEntries { get; set; } = [];

	/// <summary>
	/// 抽取轮数，为0或负数时不抽取。
	/// </summary>
	[Export]
	public NumberProvider Roll { get; set; }
}

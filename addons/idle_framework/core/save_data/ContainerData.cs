using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace IdleFramework.Core;

/// <summary>
/// 容器数据，于存档数据中作为一个容器实例的数据存储
/// </summary>
public class ContainerData : ISaveDataComponent<ContainerData>
{
	/// <summary>
	/// 该容器存储的物品，字典键为物品ID，值为物品数量
	/// </summary>
	public Dictionary<string, long> ItemCounts { get; } = [];
	
	public JObject ToJson()
	{
		JObject itemCounts = new();
		JObject result = new()
		{
			[nameof(ItemCounts)] = itemCounts,
		};
		foreach ((string itemId, long itemCount) in ItemCounts) itemCounts.Add(itemId, new JValue(itemCount));
		return result;
	}

	public static ContainerData FromJson(JObject jObject)
	{
		if (jObject == null) return null;
		ContainerData result = new();
		if (jObject.TryGetValue(nameof(ItemCounts), out JToken valueGuids) && valueGuids.Type == JTokenType.Object)
		{
			JObject itemCounts = valueGuids.Value<JObject>();
			foreach ((string itemId, JToken itemCount) in itemCounts)
			{
				if (itemCount.Type != JTokenType.Integer) continue;
				result.ItemCounts[itemId] = itemCount.Value<long>();
			}
		}
		return result;
	}

	public ContainerData Duplicate()
	{
		ContainerData duplicated = new();
		foreach ((string itemId, long itemCount) in ItemCounts)
		{
			duplicated.ItemCounts[itemId] = itemCount;
		}
		return duplicated;
	}
	
}
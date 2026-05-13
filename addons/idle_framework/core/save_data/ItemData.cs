using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace IdleFramework.Core;

/// <summary>
/// 物品数据，于容器数据中作为代表一种ID的物品实例的数据存储
/// </summary>
public class ItemData : ISaveDataComponent<ItemData>
{
	/// <summary>
	/// 物品数量
	/// </summary>
	public ulong Count { get; set; }

	/// <summary>
	/// 物品实例的GUID表(映射到SaveData的容器表、工厂表、富数据物品数据表)
	/// </summary>
	public List<Guid> Guids { get; set; } = [];
	
	public JObject ToJson()
	{
		JObject result = new()
		{
			[nameof(Count)] = new JValue(Count),
		};
		JArray richDataItemGuidsJArray = [];
		foreach (Guid guid in Guids)
		{
			richDataItemGuidsJArray.Add(new JValue(guid.ToString()));
		}
		result[nameof(Guids)] = richDataItemGuidsJArray;
		return result;
	}
	
	/// <summary>
	/// 从<c>JObject</c>解析为本存档数据组件类型并返回解析完成的实例。
	/// </summary>
	/// <param name="jObject">要解析的<c>JObject</c>。</param>
	/// <returns>解析完成的实例，如果失败则返回null。</returns>
	public static ItemData FromJson(JObject jObject)
	{
		if (jObject == null) return null;
		ItemData result = new();
		if (jObject.TryGetValue(nameof(Count), out JToken valueCount) && valueCount.Type == JTokenType.Integer) result.Count = valueCount.Value<ulong>();
		return result;
	}

	public ItemData Duplicate()
	{
		ItemData duplicated = new()
		{
			Count = Count,
		};
		duplicated.Guids.AddRange(Guids);
		return duplicated;
	}
}
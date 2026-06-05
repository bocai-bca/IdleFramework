using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace IdleFramework.Core;

/// <summary>
/// 容器编组，一种实例对象。表示一群不重复的容器GUID。
/// </summary>
public class ContainerGroup : GuidGroup, ISaveDataComponent<ContainerGroup>
{
	/// <summary>
	/// 图标物品ID
	/// </summary>
	public string IconItemID { get; set; }

	public new JObject ToJson()
	{
		JArray guidsJArray = [];
		JObject result = new()
		{
			[nameof(Guids)] = guidsJArray,
			[nameof(IconItemID)] = new JValue(IconItemID),
		};
		foreach (Guid guid in Guids) guidsJArray.Add(new JValue(guid));
		return result;
	}
	
	public new static ContainerGroup FromJson(JObject jObject)
	{
		if (jObject == null) return null;
		ContainerGroup result = new();
		if (jObject.TryGetValue(nameof(Guids), out JToken valueGuids) && valueGuids.Type == JTokenType.Array)
		{
			JArray guidsArray = valueGuids.Value<JArray>();
			foreach (JToken guidJToken in guidsArray) if (guidJToken.Type == JTokenType.Guid) result.Guids.Add(guidJToken.Value<Guid>());
		}
		if (jObject.TryGetValue(nameof(IconItemID), out JToken valueIconItem) && valueIconItem.Type == JTokenType.String)
		{
			result.IconItemID = valueIconItem.Value<string>();
		}
		return result;
	}

	public new ContainerGroup Duplicate()
	{
		ContainerGroup duplicated = new()
		{
			IconItemID = IconItemID,
		};
		foreach (Guid guid in Guids) duplicated.Guids.Add(guid);
		return duplicated;
	}
}
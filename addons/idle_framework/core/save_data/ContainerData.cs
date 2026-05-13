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
	public Dictionary<string, ulong> ItemCounts { get; } = [];
	
	public JObject ToJson()
	{
		throw new System.NotImplementedException();
	}

	public static ContainerData FromJson(JObject jObject)
	{
		throw new System.NotImplementedException();
	}

	public ContainerData Duplicate()
	{
		throw new System.NotImplementedException();
	}
	
}
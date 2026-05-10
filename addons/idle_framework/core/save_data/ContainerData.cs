using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace IdleFramework.Core;

/// <summary>
/// 容器数据，于存档数据中作为一个容器实例的数据存储
/// </summary>
public class ContainerData : ISaveDataComponent<ContainerData>
{
	/// <summary>
	/// 对该容器的ID的缓存，用来在运行时通过游戏资源获取容器注册表。不会经过序列化进入Json对象中。
	/// </summary>
	public string ID { get; set; }
	
	/// <summary>
	/// 该容器存储的物品，字典键为物品ID，值为物品数据
	/// </summary>
	public Dictionary<string, ItemData> Items { get; } = [];
	
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
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace IdleFramework.Core;

public class ContainerData : ISaveDataComponent<ContainerData>
{
	/// <summary>
	/// 容器存储的物品字典，键为物品ID，值为物品的数量
	/// </summary>
	public Dictionary<string, int> ContainedItems { get; set; } = [];
	
	public JToken ToJson()
	{
		throw new System.NotImplementedException();
	}

	public static ContainerData FromJson(JToken jToken)
	{
		throw new System.NotImplementedException();
	}

	public ContainerData Duplicate()
	{
		ContainerData duplicated = new();
		return duplicated;
	}
}
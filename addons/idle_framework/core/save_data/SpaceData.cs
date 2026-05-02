using Newtonsoft.Json.Linq;

namespace IdleFramework.Core;

/// <summary>
/// 空间数据，于存档数据中作为单个空间的数据存储
/// </summary>
public class SpaceData : ISaveDataComponent<SpaceData>
{
	/// <summary>
	/// 空间容器
	/// </summary>
	public ContainerData SpaceContainer { get; set; } = new();
	
	public JToken ToJson()
	{
		throw new System.NotImplementedException();
	}

	public static SpaceData FromJson(JToken jToken)
	{
		throw new System.NotImplementedException();
	}

	public SpaceData Duplicate()
	{
		SpaceData duplicated = new();
		return duplicated;
	}
}
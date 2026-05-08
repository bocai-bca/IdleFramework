using Newtonsoft.Json.Linq;

namespace IdleFramework.Core;

/// <summary>
/// 工厂数据，于存档数据中作为一个工厂实例的数据存储
/// </summary>
public class FactoryData : ISaveDataComponent<FactoryData>
{
	public JObject ToJson()
	{
		throw new System.NotImplementedException();
	}

	public static FactoryData FromJson(JObject jObject)
	{
		throw new System.NotImplementedException();
	}

	public FactoryData Duplicate()
	{
		throw new System.NotImplementedException();
	}
}
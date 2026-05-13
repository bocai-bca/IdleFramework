using System;
using Newtonsoft.Json.Linq;

namespace IdleFramework.Core;

/// <summary>
/// 工厂数据，于存档数据中作为一个工厂实例的数据存储
/// </summary>
public class FactoryData : ISaveDataComponent<FactoryData>
{
	/// <summary>
	/// 工厂原料需求模式
	/// </summary>
	public FactoryIngredientRequireMode FactoryMode { get; set; }
	
	public JObject ToJson()
	{
		JObject result = new()
		{
			[nameof(FactoryMode)] = new JValue(FactoryMode.ToString()),
		};
		return result;
	}

	public static FactoryData FromJson(JObject jObject)
	{
		if (jObject == null) return null;
		FactoryData result = new();
		if (jObject.TryGetValue(nameof(FactoryMode), out JToken valueMode) && valueMode.Type == JTokenType.String)
		{
			if (Enum.TryParse(valueMode.Value<string>(), out FactoryIngredientRequireMode mode)) result.FactoryMode = mode;
		}
		return result;
	}

	public FactoryData Duplicate()
	{
		FactoryData duplicated = new()
		{
			FactoryMode = FactoryMode,
		};
		return duplicated;
	}
}
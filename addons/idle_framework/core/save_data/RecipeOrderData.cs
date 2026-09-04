using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace IdleFramework.Core;

/// <summary>
/// 配方下单器数据，于存档数据中包含在一个工厂数据中作为一个配方下单器实例的数据存储
/// </summary>
public class RecipeOrderData: ISaveDataComponent<RecipeOrderData>
{
	/// <summary>
	/// 整型参数表
	/// </summary>
	public List<long> ArgumentsLong { get; init; } = [];
	
	/// <summary>
	/// 字符串参数表
	/// </summary>
	public List<string> ArgumentsString { get; init; } = [];
	
	public JObject ToJson()
	{
		JObject result = new()
		{
			[nameof(ArgumentsLong)] = new JArray(ArgumentsLong.Select(x => new JValue(x))),
			[nameof(ArgumentsString)] = new JArray(ArgumentsString.Select(x => new JValue(x))),
		};
		return result;
	}

	public static RecipeOrderData FromJson(JObject jObject)
	{
		if (jObject == null) return null;
		RecipeOrderData result = new();
		if (jObject.TryGetValue(nameof(ArgumentsLong), out JToken valueArgumentsLong) && valueArgumentsLong.Type == JTokenType.Array)
		{
			foreach (JValue jValue in valueArgumentsLong.Values<JValue>())
			{
				if (jValue.Type == JTokenType.Integer) result.ArgumentsLong.Add(jValue.Value<long>());
			}
		}
		if (jObject.TryGetValue(nameof(ArgumentsString), out JToken valueArgumentsString) && valueArgumentsString.Type == JTokenType.Array)
		{
			foreach (JValue jValue in valueArgumentsString.Values<JValue>())
			{
				if (jValue.Type == JTokenType.String) result.ArgumentsString.Add(jValue.Value<string>());
			}
		}
		return result;
	}

	public RecipeOrderData Duplicate()
	{
		RecipeOrderData duplicated = new()
		{ 
			ArgumentsLong = [..ArgumentsLong],
			ArgumentsString = [..ArgumentsString],
		};
		return duplicated;
	}
}
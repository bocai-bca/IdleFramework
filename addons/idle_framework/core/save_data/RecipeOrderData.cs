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
	public List<long> DataListLong { get; init; } = [];
	
	/// <summary>
	/// 字符串队列表
	/// </summary>
	public Queue<string> DataQueueString { get; init; } = [];
	
	public JObject ToJson()
	{
		JObject result = new()
		{
			[nameof(DataListLong)] = new JArray(DataListLong.Select(x => new JValue(x))),
			[nameof(DataQueueString)] = new JArray(DataQueueString.Select(x => new JValue(x))),
		};
		return result;
	}

	public static RecipeOrderData FromJson(JObject jObject)
	{
		if (jObject == null) return null;
		RecipeOrderData result = new();
		if (jObject.TryGetValue(nameof(DataListLong), out JToken valueArgumentsLong) && valueArgumentsLong.Type == JTokenType.Array)
		{
			foreach (JValue jValue in valueArgumentsLong.Values<JValue>())
			{
				if (jValue.Type == JTokenType.Integer) result.DataListLong.Add(jValue.Value<long>());
			}
		}
		if (jObject.TryGetValue(nameof(DataQueueString), out JToken valueArgumentsString) && valueArgumentsString.Type == JTokenType.Array)
		{
			foreach (JValue jValue in valueArgumentsString.Values<JValue>())
			{
				if (jValue.Type == JTokenType.String) result.DataQueueString.Enqueue(jValue.Value<string>());
			}
		}
		return result;
	}

	public RecipeOrderData Duplicate()
	{
		RecipeOrderData duplicated = new()
		{ 
			DataListLong = [..DataListLong],
			DataQueueString = new Queue<string>([..DataQueueString]),
		};
		return duplicated;
	}
}
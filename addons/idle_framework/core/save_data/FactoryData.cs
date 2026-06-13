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
	
	/// <summary>
	/// 当前正执行的配方的ID，如果没有在执行配方则为空字符串
	/// </summary>
	public string CurrentRecipe { get; set; } = string.Empty;
	
	/// <summary>
	/// 配方执行的开始时间
	/// </summary>
	public DateTime StartTime { get; set; }
	
	/// <summary>
	/// 配方执行的结束时间，仅适用于部分工厂模式
	/// </summary>
	public DateTime DoneTime { get; set; }
	
	/// <summary>
	/// 配方已工作的时间刻数，仅适用于部分工厂模式
	/// </summary>
	public long RecipeProcessedTicks { get; set; }
	
	/// <summary>
	/// 输入容器的GUID
	/// </summary>
	public Guid InputContainerGuid { get; set; }
	
	/// <summary>
	/// 输出容器的GUID
	/// </summary>
	public Guid OutputContainerGuid { get; set; }

	/// <summary>
	/// 该工厂是否已经开始生产
	/// </summary>
	public bool WasStarted { get; set; }
	
	/// <summary>
	/// 工厂ID缓存，由更新器赋值，不会被持久化。
	/// </summary>
	public string FactoryIdCache = string.Empty;
	
	public JObject ToJson()
	{
		JObject result = new()
		{
			[nameof(FactoryMode)] = new JValue(FactoryMode.ToString()),
			[nameof(CurrentRecipe)] = new JValue(CurrentRecipe),
			[nameof(StartTime)] = new JValue(StartTime.Ticks),
			[nameof(DoneTime)] = new JValue(DoneTime.Ticks),
			[nameof(InputContainerGuid)] = new JValue(InputContainerGuid),
			[nameof(OutputContainerGuid)] = new JValue(OutputContainerGuid),
			[nameof(WasStarted)] = new JValue(WasStarted),
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
		if (jObject.TryGetValue(nameof(CurrentRecipe), out JToken valueCurrentRecipe) && valueCurrentRecipe.Type == JTokenType.String)
		{
			result.CurrentRecipe = valueCurrentRecipe.Value<string>();
		}
		if (jObject.TryGetValue(nameof(StartTime), out JToken valueStartTime) && valueStartTime.Type == JTokenType.Integer)
		{
			result.StartTime = new DateTime(valueStartTime.Value<long>());
		}
		if (jObject.TryGetValue(nameof(DoneTime), out JToken valueDoneTime) && valueDoneTime.Type == JTokenType.Integer)
		{
			result.DoneTime = new DateTime(valueDoneTime.Value<long>());
		}
		if (jObject.TryGetValue(nameof(InputContainerGuid), out JToken valueInputContainerGuid) && valueInputContainerGuid.Type == JTokenType.Guid)
		{
			result.InputContainerGuid = valueInputContainerGuid.Value<Guid>();
		}
		if (jObject.TryGetValue(nameof(InputContainerGuid), out JToken valueOutputContainerGuid) && valueOutputContainerGuid.Type == JTokenType.Guid)
		{
			result.OutputContainerGuid = valueOutputContainerGuid.Value<Guid>();
		}

		if (jObject.TryGetValue(nameof(WasStarted), out JToken valueWasStarted) && valueWasStarted.Type == JTokenType.Boolean)
		{
			result.WasStarted = valueWasStarted.Value<bool>();
		}
		return result;
	}

	public FactoryData Duplicate()
	{
		FactoryData duplicated = new()
		{
			FactoryMode = FactoryMode,
			CurrentRecipe = CurrentRecipe,
			StartTime = StartTime,
			InputContainerGuid = InputContainerGuid,
			OutputContainerGuid = OutputContainerGuid,
			WasStarted = WasStarted,
		};
		return duplicated;
	}
}
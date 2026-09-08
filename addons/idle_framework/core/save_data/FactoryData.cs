using System;
using Newtonsoft.Json.Linq;

namespace IdleFramework.Core;

/// <summary>
/// 工厂数据，于存档数据中作为一个工厂实例的数据存储
/// </summary>
public class FactoryData : ISaveDataComponent<FactoryData>
{
	/// <summary>
	/// 工厂配方下单器
	/// </summary>
	public RecipeOrderData RecipeOrderData { get; set; }
	
	/// <summary>
	/// 工厂原料需求模式
	/// </summary>
	public FactoryIngredientRequireMode FactoryMode { get; set; }
	
	/// <summary>
	/// 当前正执行的配方的ID，如果没有在执行配方则为空字符串
	/// </summary>
	public string CurrentRecipe { get; set; } = string.Empty;
	
	/// <summary>
	/// 该工厂是否已经开始生产，是对<c>CurrentRecipe</c>的检查的封装。
	/// </summary>
	public bool WasStarted => CurrentRecipe != string.Empty;
	
	/// <summary>
	/// 配方执行的开始时间
	/// </summary>
	public DateTime RecipeStartTime { get; set; }
	
	/// <summary>
	/// 配方已工作的时间刻数，仅适用于部分工厂模式
	/// </summary>
	public long RecipeWorkedTicks { get; set; }
	
	/// <summary>
	/// 配方总共所需工作的时间刻数。
	/// </summary>
	public long RecipeRequiredTicks { get; set; }
	
	/// <summary>
	/// 该工厂正在执行的配方的完成度百分比。
	/// </summary>
	public float RecipeWorkingPercent => (float)RecipeWorkedTicks / RecipeRequiredTicks;
	
	/// <summary>
	/// 该工厂正在执行的配方剩余所需的工作时间刻数。
	/// </summary>
	public long RecipeRemainingTicks => RecipeRequiredTicks - RecipeWorkedTicks;
	
	/// <summary>
	/// 输入容器的GUID
	/// </summary>
	public Guid InputContainerGuid { get; set; }
	
	/// <summary>
	/// 输出容器的GUID
	/// </summary>
	public Guid OutputContainerGuid { get; set; }
	
	public JObject ToJson()
	{
		JObject result = new()
		{
			[nameof(RecipeOrderData)] = RecipeOrderData.ToJson(),
			[nameof(FactoryMode)] = new JValue(FactoryMode.ToString()),
			[nameof(CurrentRecipe)] = new JValue(CurrentRecipe),
			[nameof(RecipeStartTime)] = new JValue(RecipeStartTime.Ticks),
			[nameof(RecipeWorkedTicks)] = new JValue(RecipeWorkedTicks),
			[nameof(RecipeRequiredTicks)] = new JValue(RecipeRequiredTicks),
			[nameof(InputContainerGuid)] = new JValue(InputContainerGuid),
			[nameof(OutputContainerGuid)] = new JValue(OutputContainerGuid),
		};
		return result;
	}

	public static FactoryData FromJson(JObject jObject)
	{
		if (jObject == null) return null;
		FactoryData result = new();
		if (jObject.TryGetValue(nameof(RecipeOrderData), out JToken valueRecipeOrderData) && valueRecipeOrderData.Type == JTokenType.Object)
		{
			if (valueRecipeOrderData is JObject valueRecipeOrderDataJObject)
			{
				result.RecipeOrderData = RecipeOrderData.FromJson(valueRecipeOrderDataJObject);
			}
		}
		if (jObject.TryGetValue(nameof(FactoryMode), out JToken valueMode) && valueMode.Type == JTokenType.String)
		{
			if (Enum.TryParse(valueMode.Value<string>(), out FactoryIngredientRequireMode mode)) result.FactoryMode = mode;
		}
		if (jObject.TryGetValue(nameof(CurrentRecipe), out JToken valueCurrentRecipe) && valueCurrentRecipe.Type == JTokenType.String)
		{
			result.CurrentRecipe = valueCurrentRecipe.Value<string>();
		}
		if (jObject.TryGetValue(nameof(RecipeStartTime), out JToken valueStartTime) && valueStartTime.Type == JTokenType.Integer)
		{
			result.RecipeStartTime = new DateTime(valueStartTime.Value<long>());
		}
		if (jObject.TryGetValue(nameof(RecipeWorkedTicks), out JToken valueRecipeWorkedTicks) && valueRecipeWorkedTicks.Type == JTokenType.Integer)
		{
			result.RecipeWorkedTicks = valueRecipeWorkedTicks.Value<long>();
		}
		if (jObject.TryGetValue(nameof(RecipeRequiredTicks), out JToken valueRecipeRequiredTicks) && valueRecipeRequiredTicks.Type == JTokenType.Integer)
		{
			result.RecipeRequiredTicks = valueRecipeRequiredTicks.Value<long>();
		}
		if (jObject.TryGetValue(nameof(InputContainerGuid), out JToken valueInputContainerGuid) && valueInputContainerGuid.Type == JTokenType.Guid)
		{
			result.InputContainerGuid = valueInputContainerGuid.Value<Guid>();
		}
		if (jObject.TryGetValue(nameof(InputContainerGuid), out JToken valueOutputContainerGuid) && valueOutputContainerGuid.Type == JTokenType.Guid)
		{
			result.OutputContainerGuid = valueOutputContainerGuid.Value<Guid>();
		}
		return result;
	}

	public FactoryData Duplicate()
	{
		FactoryData duplicated = new()
		{
			RecipeOrderData = RecipeOrderData.Duplicate(),
			FactoryMode = FactoryMode,
			CurrentRecipe = CurrentRecipe,
			RecipeStartTime = RecipeStartTime,
			RecipeWorkedTicks = RecipeWorkedTicks,
			RecipeRequiredTicks = RecipeRequiredTicks,
			InputContainerGuid = InputContainerGuid,
			OutputContainerGuid = OutputContainerGuid,
		};
		return duplicated;
	}
}
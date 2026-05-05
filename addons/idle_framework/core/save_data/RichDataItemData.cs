using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace IdleFramework.Core;

/// <summary>
/// 富数据物品的数据类，其本质上是一个<c>Dictionary&lt;string, object&gt;</c>，但提供了一定的封装性，同时对进出数据的类型做了检测，提供从Json正反转换的功能。
/// 对于字典需求，直接将本类型实例当做字典用即可；对于字符串、整数、浮点数、布尔型这样的值类型，可以用键存储于本类型实例中；对于列表需求，本类型可以持有<c>List&lt;RichDataItemData&gt;</c>。
/// </summary>
public class RichDataItemData : ISaveDataComponent<RichDataItemData>
{
	/// <summary>
	/// 该实例存放的数据，键为字符串，值为数据，虽然值类型是<c>object</c>，但实际可存储的类型有限，必须是如int、double、string、JObject、JArray这种易于正反序列化的类型
	/// </summary>
	private Dictionary<string, object> Data { get; set; } = [];
	
	/// <summary>
	/// 转换到Json。
	/// </summary>
	/// <returns>转换后的<c>JToken</c>(实际上是<c>JObject</c>)。</returns>
	public JToken ToJson()
	{
		JObject jObject = new();
		foreach ((string key, object value) in Data)
		{
			switch (value)
			{
				case RichDataItemData richDataItemData: // RichDataItemData会转换为JObject
					jObject[key] = richDataItemData.ToJson();
					continue;
				case List<RichDataItemData> list: // List<RichDataItemData>会转换为JArray
					jObject[key] = list.ToJson();
					continue;
				case string valueString: // 转换为JValue
					jObject[key] = new JValue(valueString);
					continue;
				case long valueLong: // 转换为JValue
					jObject[key] = new JValue(valueLong);
					continue;
				case bool valueBool: // 转换为JValue
					jObject[key] = new JValue(valueBool);
					continue;
				case double valueDouble: // 转换为JValue
					jObject[key] = new JValue(valueDouble);
					continue;
				default: // 不支持的类型报错
					Logger.LogError(Localization.Tr("log.error.rich_data_item_data.cannot_convert_an_unsupported_type_data_into_a_json_token"));
					continue;
			}
		}
		return jObject;
	}

	/// <summary>
	/// 从Json解析。
	/// </summary>
	/// <param name="jObject">待解析的<c>JObject</c>实例。</param>
	/// <returns>解析好的<c>RichDataItemData</c>实例。</returns>
	public static RichDataItemData FromJson(JObject jObject)
	{
		if (jObject == null) return null;
		RichDataItemData result = new();
		foreach ((string key, JToken value) in jObject)
		{
			switch (value.Type) // 检查子值的Json类型
			{
				case JTokenType.Object: // JObject是RichDataItemData，直接解析数据
					JObject childJObject = (JObject)value;
					result.Data[key] = FromJson(childJObject);
					continue;
				case JTokenType.Array: // JArray是List<RichDataItemData>或List<string>
					JArray childJArray = (JArray)value;
					List<RichDataItemData> listRdi = [];
					if (childJArray.Count <= 0) continue;
					JTokenType childType = childJArray[0].Type;
					if (childType == JTokenType.String)
					{
						List<string> listString = [];
						foreach (JToken child in childJArray)
						{
							listString.Add(child.Value<string>());
						}
						result.Data[key] = listString;
						continue;
					}
					if (childType == JTokenType.Object)
					{
						foreach (JToken child in childJArray)
						{
							listRdi.Add(FromJson((JObject)child));
						}
						result.Data[key] = listRdi;
						continue;
					}
					Logger.LogError(
						string.Format(
							Localization.Tr("log.error.rich_data_item_data.unsupported_jtoken_type_of_child_token_of_jarray"),
							childType.ToString()
							)
						);
					continue;
				case JTokenType.Integer:
					JValue childJValueInt = (JValue)value;
					if (childJValueInt.Value is int valueInt) result.Data[key] = (long)valueInt;
					else result.Data[key] = childJValueInt.Value;
					continue;
				case JTokenType.Float:
					JValue childJValueFloat = (JValue)value;
					if (childJValueFloat.Value is float valueFloat) result.Data[key] = (double)valueFloat;
					else result.Data[key] = childJValueFloat.Value;
					continue;
				case JTokenType.String:
				case JTokenType.Boolean:
					JValue childJValue = (JValue)value;
					result.Data[key] = childJValue.Value;
					continue;
				default:
					Logger.LogError(Localization.Tr("log.error.rich_data_item_data.cannot_convert_an_unsupported_jtoken_type_into_a_rich_data_item_data"));
					continue;
			}
		}
		return result;
	}

	/// <summary>
	/// 复制该<c>RichDataItemData</c>实例。
	/// </summary>
	/// <returns>复制好的新实例。</returns>
	public RichDataItemData Duplicate()
	{
		if (recursionLock)
		{
			Logger.LogError(Localization.Tr("log.error.rich_data_item_data.unexcepted_call_blocked_by_recursion_lock"));
			return null;
		}
		recursionLock = true;
		RichDataItemData duplicated = new();
		try
		{
			foreach ((string key, object value) in Data)
			{
				switch (value)
				{
					case RichDataItemData richDataItemData:
						duplicated.Data[key] = richDataItemData.Duplicate();
						continue;
					case List<RichDataItemData> list:
						duplicated.Data[key] = list.Duplicate();
						continue;
					case List<string> listString:
						duplicated.Data[key] = new List<string>(listString);
						continue;
					default:
						duplicated.Data[key] = value;
						break;
				}
			}
		}
		catch (Exception e)
		{
			Logger.LogError(string.Format(Localization.Tr("log.error.rich_data_item_data.catched_exception_on_calling_rich_data_item_data_list_duplicate"), e));
			recursionLock = false;
			throw;
		}
		recursionLock = false;
		return duplicated;
	}

	/// <summary>
	/// 通过键获取本实例的值。
	/// </summary>
	/// <param name="key">要访问的键。</param>
	/// <param name="value">返回的值。</param>
	/// <typeparam name="T">要获取的值的类型，如果对应键不属于给定类型则会返回该类型的默认值并使方法返回<c>false</c>。如果不能提前确定想要获取的值的类型，可以在此指定<c>object</c>来使其无论如何都返回值。</typeparam>
	/// <returns>获取的成功与否，在本实例不存在对应键时和对应键的类型不符合提供的泛型实参的值时返回<c>false</c>。</returns>
	public bool GetData<T>(string key, out T value)
	{
		if (Data.TryGetValue(key, out object obj))
		{
			if (obj is T valueGot)
			{
				value = valueGot;
				return true;
			}
		}
		value = default;
		return false;
	}

	/// <summary>
	/// 为本实例的一个键设置值，当重复时覆盖值。如果给定的类型不可以存储，则会丢弃并返回<c>false</c>。
	/// </summary>
	/// <param name="key">要存入的键。</param>
	/// <param name="value">要存入的数据，只支持以下类型：<c>null</c>、<c>long</c>、<c>bool</c>、<c>double</c>、<c>string</c>、<c>List&lt;string&gt;</c>、<c>List&lt;RichDataItemData&gt;</c>、<c>RichDataItemData</c>。其中<c>null</c>的行为比较特殊，它相当于调用<c>RemoveKey(key)</c>，如果对应键不存在，也会返回<c>false</c>。</param>
	/// <returns>成功与否。</returns>
	public bool SetData(string key, object value)
	{
		switch (value)
		{
			case null:
				return RemoveKey(key);
			case long valueLong:
				Data[key] = valueLong;
				return true;
			case bool valueBool:
				Data[key] = valueBool;
				return true;
			case string valueString:
				Data[key] = valueString;
				return true;
			case double valueDouble:
				Data[key] = valueDouble;
				return true;
			case List<RichDataItemData> valueList:
				Data[key] = valueList;
				return true;
			case List<string> valueListString:
				Data[key] = valueListString;
				return true;
			case RichDataItemData valueRichData:
				Data[key] = valueRichData;
				return true;
			default:
				return false;
		}
	}

	/// <summary>
	/// 检查给定键是否存在。
	/// </summary>
	/// <param name="key">要查询的键。</param>
	/// <returns>查询结果。</returns>
	public bool ContainsKey(string key) => Data.ContainsKey(key);
	
	/// <summary>
	/// 移除给定键。
	/// </summary>
	/// <param name="key">要移除的键。</param>
	/// <returns>移除结果。</returns>
	public bool RemoveKey(string key) => Data.Remove(key);

	private bool recursionLock;
}

public static class RichDataItemDataExtension
{
	/// <param name="list">待复制的列表。</param>
	extension(List<RichDataItemData> list)
	{
		/// <summary>
		/// 复制富数据列表实例。
		/// </summary>
		/// <returns>复制好的列表。</returns>
		public List<RichDataItemData> Duplicate()
		{
			List<RichDataItemData> duplicated = new(list.Count);
			duplicated.AddRange(list.Select(t => t.Duplicate()));
			return duplicated;
		}

		/// <summary>
		/// 转换到Json。
		/// </summary>
		/// <returns>转换后的<c>JArray</c>。</returns>
		public JArray ToJson()
		{
			JArray result = [];
			foreach (RichDataItemData richDataItemData in list)
			{
				result.Add(richDataItemData.ToJson());
			}
			return result;
		}
	}
}
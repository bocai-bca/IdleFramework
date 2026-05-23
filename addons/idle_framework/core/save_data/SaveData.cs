using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IdleFramework.Core;

/// <summary>
/// 存档数据，代表一个游戏的一个存档槽位的一瞬状态数据。
/// <c>SaveData</c>类型本身并不线程安全，一些多线程操作类会持有各自任务所需或作为任务结果的<c>SaveData</c>实例，请务必遵守文档的说明，注意防范线程安全问题。
/// </summary>
public class SaveData
{
	/// <summary>
	/// 记录本存档对应的游戏ID
	/// </summary>
	public string GameID { get; set; } = "";
	
	/// <summary>
	/// 记录本存档对应的游戏版本号
	/// </summary>
	public int GameVersion { get; set; } = 0;

	/// <summary>
	/// 记录本存档上一次更新的时间
	/// </summary>
	public long LastUpdateUtcTick { get; set; } = 0L;
	
	/// <summary>
	/// 空间数据字典，键为空间ID，值为对应的空间数据
	/// </summary>
	public Dictionary<string, SpaceData> SpaceDatas { get; set; } = [];
	
	/// <summary>
	/// 容器实例表
	/// </summary>
	public Dictionary<Guid, ContainerData> ContainerDatas { get; set; } = [];
	
	/// <summary>
	/// 富数据物品实例表
	/// </summary>
	public Dictionary<Guid, RichDataItemData> RichDataItems { get; set; } = [];

	/// <summary>
	/// 工厂实例表
	/// </summary>
	public Dictionary<Guid, FactoryData> FactoryDatas { get; set; } = [];
	
	/// <summary>
	/// 实例物品的名称表
	/// </summary>
	public Dictionary<Guid, string> InstanceNames { get; set; } = [];
	
	/// <summary>
	/// 将本实例转换为Json对象。
	/// </summary>
	/// <returns>转换后的Json对象。</returns>
	public JObject ToJson()
	{
		JObject spaceDatasJObject = new();
		JObject containerDatasJObject = new();
		JObject factoryDatasJObject = new();
		JObject richDataItemsJObject = new();
		JObject instanceNamesJObject = new();
		JObject root = new()
		{
			[nameof(GameID)] = GameID,
			[nameof(LastUpdateUtcTick)] = LastUpdateUtcTick,
			[nameof(SpaceDatas)] = spaceDatasJObject,
			[nameof(ContainerDatas)] = containerDatasJObject,
			[nameof(FactoryDatas)] = factoryDatasJObject,
			[nameof(RichDataItems)] = richDataItemsJObject,
			[nameof(InstanceNames)] = instanceNamesJObject,
		};
		foreach ((string key, SpaceData spaceData) in SpaceDatas) spaceDatasJObject.Add(key, spaceData.ToJson());
		foreach ((Guid guid, ContainerData containerData) in ContainerDatas) containerDatasJObject.Add(guid.ToString(), containerData.ToJson());
		foreach ((Guid guid, FactoryData factoryData) in FactoryDatas) factoryDatasJObject.Add(guid.ToString(), factoryData.ToJson());
		foreach ((Guid guid, RichDataItemData richDataItem) in RichDataItems) richDataItemsJObject.Add(guid.ToString(), richDataItem.ToJson());
		foreach ((Guid guid, string instanceName) in InstanceNames) instanceNamesJObject.Add(guid.ToString(), new JValue(instanceName));
		return root;
	}
	
	/// <summary>
	/// 将本实例序列化为Json文本。
	/// </summary>
	/// <returns>转换后的文本。</returns>
	[Pure]
	public string ToJsonText()
	{
		return ToJson().ToString(Formatting.Indented);
	}

	/// <summary>
	/// 从本实例复制一个独立的新实例。
	/// </summary>
	/// <param name="deep">是否深度复制。</param>
	/// <returns>复制出来的新实例。</returns>
	[Pure]
	public SaveData Duplicate(bool deep = true)
	{
		return Duplicate(this, deep);
	}

	/// <summary>
	/// 从给定的原始实例复制一个独立的新实例。
	/// </summary>
	/// <param name="originalData">要复制的原始实例。</param>
	/// <param name="deep">是否深度复制。</param>
	/// <returns>复制出来的新实例。</returns>
	[Pure]
	public static SaveData Duplicate([DisallowNull] SaveData originalData, bool deep = true)
	{
		SaveData newInstance = new()
		{
			GameID = originalData.GameID,
			GameVersion = originalData.GameVersion,
			LastUpdateUtcTick = originalData.LastUpdateUtcTick,
			SpaceDatas = originalData.SpaceDatas,
			RichDataItems = originalData.RichDataItems,
		};
		if (deep)
		{
			newInstance.SpaceDatas = [];
			foreach ((string key, SpaceData originalSpaceData) in originalData.SpaceDatas)
			{
				SpaceData newSpaceData = originalSpaceData.Duplicate();
				if (newSpaceData is null) continue;
				newInstance.SpaceDatas[key] = newSpaceData;
			}
			newInstance.ContainerDatas = [];
			foreach ((Guid guid, ContainerData originalContainerData) in originalData.ContainerDatas)
			{
				ContainerData newContainerData = originalContainerData.Duplicate();
				if (newContainerData is null) continue;
				newInstance.ContainerDatas[guid] = newContainerData;
			}
			newInstance.FactoryDatas = [];
			foreach ((Guid guid, FactoryData originalFactoryData) in originalData.FactoryDatas)
			{
				FactoryData newFactoryData = originalFactoryData.Duplicate();
				if (newFactoryData is null) continue;
				newInstance.FactoryDatas[guid] = newFactoryData;
			}
			newInstance.RichDataItems = [];
			foreach ((Guid guid, RichDataItemData originalRichDataItem) in originalData.RichDataItems)
			{
				RichDataItemData newRichDataItemData = originalRichDataItem.Duplicate();
				if (newRichDataItemData is null) continue;
				newInstance.RichDataItems[guid] = newRichDataItemData;
			}
		}
		return newInstance;
	}
	
	/// <summary>
	/// 将Json对象解析为<c>SaveData</c>实例。
	/// </summary>
	/// <param name="jObject">待解析的Json对象。</param>
	/// <returns>解析完毕的<c>SaveData</c>实例。</returns>
	public static SaveData FromJson([DisallowNull] JObject jObject)
	{
		SaveData result = new();
		if (jObject.Value<string>("GameID") is { } valueGameID) result.GameID = valueGameID;
		if (jObject.Value<long>("LastUpdateUtcTick") is { } valueLastUpdateUtcTick) result.LastUpdateUtcTick = valueLastUpdateUtcTick;
		if (jObject.GetValue(nameof(SpaceDatas)) is { Type: JTokenType.Object } jTokenSpaceDatas)
		{
			foreach ((string spaceId, JToken jToken) in (JObject)jTokenSpaceDatas)
			{
				if (jToken is not { Type: JTokenType.Object }) continue;
				if (SpaceData.FromJson(jToken as JObject) is { } valueSpaceData) result.SpaceDatas[spaceId] = valueSpaceData;
			}
		}
		if (jObject.GetValue(nameof(ContainerDatas)) is { Type: JTokenType.Object } jTokenContainerDatas)
		{
			foreach ((string containerGuid, JToken jToken) in (JObject)jTokenContainerDatas)
			{
				if (jToken is not { Type: JTokenType.Object }) continue;
				if (!Guid.TryParse(containerGuid, out Guid guid)) continue;
				if (ContainerData.FromJson(jToken as JObject) is { } valueContainerData) result.ContainerDatas[guid] = valueContainerData;
			}
		}
		if (jObject.GetValue(nameof(FactoryDatas)) is { Type: JTokenType.Object } jTokenFactoryDatas)
		{
			foreach ((string containerGuid, JToken jToken) in (JObject)jTokenFactoryDatas)
			{
				if (jToken is not { Type: JTokenType.Object }) continue;
				if (!Guid.TryParse(containerGuid, out Guid guid)) continue;
				if (FactoryData.FromJson(jToken as JObject) is { } valueFactoryData) result.FactoryDatas[guid] = valueFactoryData;
			}
		}
		if (jObject.GetValue(nameof(RichDataItems)) is { Type: JTokenType.Object } jTokenRichItemDatas)
		{
			foreach ((string containerGuid, JToken jToken) in (JObject)jTokenRichItemDatas)
			{
				if (jToken is not { Type: JTokenType.Object }) continue;
				if (!Guid.TryParse(containerGuid, out Guid guid)) continue;
				if (RichDataItemData.FromJson(jToken as JObject) is { } valueRichItemData) result.RichDataItems[guid] = valueRichItemData;
			}
		}
		return result;
	}
	
	/// <summary>
	/// 从Json文本解析为<c>SaveData</c>实例。
	/// </summary>
	/// <param name="jsonText">待解析的Json文本。</param>
	/// <returns>解析完毕的<c>SaveData</c>实例。</returns>
	public static SaveData ParseFromJsonText(string jsonText)
	{
		return JToken.Parse(jsonText) is JObject jObject ? FromJson(jObject) : null;
	}
}
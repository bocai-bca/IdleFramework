using System;
using System.Collections.Generic;
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
	/// 富数据物品实例表
	/// </summary>
	public Dictionary<Guid, RichDataItemData> RichDataItems { get; set; } = [];

	/// <summary>
	/// 自动富数据物品索引缓存表
	/// </summary>
	public List<Guid> AutoRDIs { get; set; } = [];
	
	/// <summary>
	/// 将本实例转换为Json对象。
	/// </summary>
	/// <returns>转换后的Json对象。</returns>
	public JObject ToJson()
	{
		JObject root = new()
		{
			[nameof(GameID)] = GameID,
			[nameof(LastUpdateUtcTick)] = LastUpdateUtcTick,
		};
		return root;
	}
	
	/// <summary>
	/// 将本实例序列化为Json文本。
	/// </summary>
	/// <returns>转换后的文本。</returns>
	public string ToJsonText()
	{
		return ToJson().ToString(Formatting.Indented);
	}

	/// <summary>
	/// 从本实例复制一个独立的新实例。
	/// </summary>
	/// <param name="deep"></param>
	/// <returns>复制出来的新实例。</returns>
	public SaveData Duplicate(bool deep = true)
	{
		return Duplicate(this, deep);
	}

	/// <summary>
	/// 从给定的原始实例复制一个独立的新实例。
	/// </summary>
	/// <param name="originalData">要复制的原始实例。</param>
	/// <param name="deep"></param>
	/// <returns>复制出来的新实例。</returns>
	public static SaveData Duplicate(SaveData originalData, bool deep = true)
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
	public static SaveData FromJson(JObject jObject)
	{
		if (jObject == null) return null;
		SaveData result = new();
		if (jObject.Value<string>("GameID") is { } valueGameID) result.GameID = valueGameID;
		if (jObject.Value<long>("LastUpdateUtcTick") is { } valueLastUpdateUtcTick) result.LastUpdateUtcTick = valueLastUpdateUtcTick;
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

	/// <summary>
	/// 从给定游戏资源中获取给定ID的空间注册表项实例化一个空间数据实例，该空间实例以及内部嵌套的富数据对象会自动添加到本<c>SaveData</c>实例中。
	/// </summary>
	/// <param name="spaceId">空间ID。</param>
	/// <param name="gameResource">游戏资源，用来获取各注册表项。</param>
	/// <returns>新初始化的空间数据实例。</returns>
	public void InstantiateSpaceRegistryObject(string spaceId, GameResource gameResource)
	{
		SpaceRegistryObject spaceRegistryObject = gameResource.SpaceRegistry[spaceId];
		SpaceData newSpace = new()
		{
			SpaceContainerGuid = Guid.NewGuid(),
		};
		RichDataItemData spaceContainerData = new();
		spaceContainerData.PlaceContainer();
		foreach ((string itemId, int itemCount) in spaceRegistryObject.PrefillItems)
		{
			if (gameResource.ItemRegistry.ContainsKey(itemId)) spaceContainerData.SetData(itemId, itemCount);
			else
			{
				Logger.LogError(string.Format(Localization.Tr("log.error.save_data.item_id_givened_in_space_registry_object_is_not_found_in_item_registry"), itemId, spaceId));
				continue;
			}
			if (gameResource.ContainerRegistry.TryGetValue(itemId, out ContainerRegistryObject containerRegistryObject))
			{
				
			}
		}
		RichDataItems[newSpace.SpaceContainerGuid] = spaceContainerData;
		SpaceDatas[spaceId] = newSpace;
	}
}
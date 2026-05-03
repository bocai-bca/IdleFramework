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
	public RichDataItemData SpaceContainer { get; set; } = new();
	
	// TODO 应该转移，把这个空间容器的富数据物品数据转移到SaveData里的全局唯一的富数据物品池，在这里只持有这个富数据物品的GUID
	
	public JToken ToJson()
	{
		throw new System.NotImplementedException();
	}

	public static SpaceData FromJson(JObject jObject)
	{
		throw new System.NotImplementedException();
	}


	/// <summary>
	/// 复制该空间数据实例。
	/// </summary>
	/// <returns>复制出来的新实例。</returns>
	public SpaceData Duplicate()
	{
		SpaceData duplicated = new()
		{
			SpaceContainer = SpaceContainer.Duplicate(),
		};
		return duplicated;
	}
	
	/// <summary>
	/// 从空间注册表项初始化为一个空间数据实例。
	/// </summary>
	/// <param name="spaceRegistryObject">需要参考的空间注册表项。</param>
	/// <returns>新初始化的空间数据实例。</returns>
	public static SpaceData InitFromSpaceRegistryObject(SpaceRegistryObject spaceRegistryObject)
	{
		SpaceData newData = new();
		foreach ((string itemId, int itemCount) in spaceRegistryObject.PrefillItems)
		{
			newData.SpaceContainer.SetData(itemId, itemCount);
		}
		return newData;
	}
}
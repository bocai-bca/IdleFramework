using System;
using System.Collections.Generic;
using System.Threading;

namespace IdleFramework.Core;

/// <summary>
/// 存档数据辅助器，是对<c>SaveData</c>的操作工具。提供针对单一存档中部分数据为单位的API，必须搭配游戏资源使用。
/// 本类型提供的方法是线程安全的，每个涉及存档数据的操作都会包含锁，如果想要避免因互斥锁导致的性能下降，可以考虑复制<c>SaveData</c>然后通过它创建新的<c>SaveDataHelper</c>实例。
/// 请避免在该<c>SaveData</c>不再可用时访问通过它创建的本类实例的方法，否则将发生空引用异常。
/// </summary>
public class SaveDataHelper(GameResource targetGameResource, SaveData targetSaveData)
{
	/// <summary>
	/// 目标游戏资源。注意，直接获取本属性时，其提供的值本身不是线程安全的。
	/// </summary>
	public GameResource UsingGameResource { get; } = targetGameResource;
	
	/// <summary>
	/// 目标存档数据。注意，直接获取本属性时，其提供的值本身不是线程安全的。
	/// </summary>
	public SaveData UsingSaveData { get; } = targetSaveData;

	/// <summary>
	/// 获取存档数据的上次更新的UTC时间刻。
	/// </summary>
	/// <returns>存档数据的<c>LastUpdateUtcTick</c>属性。</returns>
	public long GetLastUpdateUtcTick()
	{
		lock (_lock)
		{
			return UsingSaveData.LastUpdateUtcTick;
		}
	}
	
	/// <summary>
	/// 将给定实例物品对象添加到存档数据并返回GUID，如果不指定自定义的GUID将随机创建新的GUID。
	/// </summary>
	/// <param name="containerData">要添加的容器数据。</param>
	/// <param name="guid">要使用的GUID，可以使用<c>Guid.NewGuid()</c>创建新GUID，或者不使用此项参数(保持此项参数的值为default)来自动创建新的GUID。</param>
	public Guid AddInstanceObject(ContainerData containerData, Guid guid = default)
	{
		if (guid == Guid.Empty) guid = Guid.NewGuid();
		lock (_lock) UsingSaveData.ContainerDatas[guid] = containerData;
		return guid;
	}
	
	/// <summary>
	/// 将给定实例物品对象添加到存档数据并返回GUID，如果不指定自定义的GUID将随机创建新的GUID。
	/// </summary>
	/// <param name="factoryData">要添加的工厂数据。</param>
	/// <param name="guid">要使用的GUID，可以使用<c>Guid.NewGuid()</c>创建新GUID，或者不使用此项参数(保持此项参数的值为default)来自动创建新的GUID。</param>
	public Guid AddInstanceObject(FactoryData factoryData, Guid guid = default)
	{
		if (guid == Guid.Empty) guid = Guid.NewGuid();
		lock (_lock) UsingSaveData.FactoryDatas[guid] = factoryData;
		return guid;
	}

	/// <summary>
	/// 将给定实例物品对象添加到存档数据并返回GUID，如果不指定自定义的GUID将随机创建新的GUID。
	/// </summary>
	/// <param name="richDataItemData">要添加的富数据物品数据。</param>
	/// <param name="guid">要使用的GUID，可以使用<c>Guid.NewGuid()</c>创建新GUID，或者不使用此项参数(保持此项参数的值为default)来自动创建新的GUID。</param>
	public Guid AddInstanceObject(RichDataItemData richDataItemData, Guid guid = default)
	{
		if (guid == Guid.Empty) guid = Guid.NewGuid();
		lock (_lock) UsingSaveData.RichDataItems[guid] = richDataItemData;
		return guid;
	}

	/// <summary>
	/// 从注册表实例化实例数据。
	/// </summary>
	/// <param name="containerRegistryObject">想要实例化的容器注册表项。</param>
	/// <returns>实例化的新容器数据。</returns>
	public ContainerData InstantiateRegistryObject(ContainerRegistryObject containerRegistryObject)
	{
		return new ContainerData();
	}

	/// <summary>
	/// 从注册表实例化实例数据。
	/// </summary>
	/// <param name="factoryRegistryObject">想要实例化的工厂注册表项。</param>
	/// <returns>实例化的新工厂数据。</returns>
	public FactoryData InstantiateRegistryObject(FactoryRegistryObject factoryRegistryObject)
	{
		FactoryData result = new()
		{
			FactoryMode = factoryRegistryObject.IngredientRequireMode,
		};
		return result;
	}
	
	/// <summary>
	/// 初始化整个存档，如果现有存档已有数据将不会清空，字典数据进行覆盖操作。
	/// </summary>
	public void InitWholeSave()
	{
		InitMetaData();
		InitSpaces();
	}

	/// <summary>
	/// 初始化存档的元数据部分
	/// </summary>
	public void InitMetaData()
	{
		lock (_lock)
		{
			UsingSaveData.GameID = UsingGameResource.GameID;
			UsingSaveData.GameVersion = UsingGameResource.GameVersion;
			UsingSaveData.LastUpdateUtcTick = TimeHelper.GetUtcNowTick();
		}
	}
	
	/// <summary>
	/// 初始化空间，通过游戏资源的空间注册表为存档数据的空间表创建空间数据实例，同时创建空间容器，并添加到容器表。
	/// </summary>
	public void InitSpaces()
	{
		foreach ((string spaceId, SpaceRegistryObject spaceRegistryObject) in UsingGameResource.SpaceRegistry)
		{
			SpaceData newSpaceData = new();
			ContainerData newSpaceContainerData = new();
			foreach ((string itemId, ulong itemCount) in spaceRegistryObject.PrefillItems) //设置预装物品
			{
				newSpaceContainerData.ItemCounts[itemId] = itemCount; //设置物品数量
				bool hasContainerRegistry = UsingGameResource.ContainerRegistry.TryGetValue(itemId, out ContainerRegistryObject prefillItemContainerRegistryObject);
				bool hasFactoryRegistry = UsingGameResource.FactoryRegistry.TryGetValue(itemId, out FactoryRegistryObject prefillItemFactoryRegistryObject);
				if (!hasContainerRegistry && !hasFactoryRegistry) continue;
				List<Guid> prefillInstanceGuids = [];
				for (ulong i = 0; i < itemCount; i++)
				{
					Guid currentInstanceGuid = Guid.NewGuid();
					if (hasContainerRegistry) AddInstanceObject(InstantiateRegistryObject(prefillItemContainerRegistryObject), currentInstanceGuid);
					if (hasFactoryRegistry) AddInstanceObject(InstantiateRegistryObject(prefillItemFactoryRegistryObject), currentInstanceGuid);
					prefillInstanceGuids.Add(currentInstanceGuid);
				}
				newSpaceData.InstanceItemGuids[itemId] = prefillInstanceGuids;
			}
			newSpaceData.SpaceContainerGuid = AddInstanceObject(newSpaceContainerData);
			lock (_lock)
			{
				UsingSaveData.SpaceDatas[spaceId] = newSpaceData;
			}
		}
	}
	
	/// <summary>
	/// 保护线程安全用的互斥锁，使用时应尽量缩小原子单元。
	/// </summary>
	private readonly Lock _lock = new();
}
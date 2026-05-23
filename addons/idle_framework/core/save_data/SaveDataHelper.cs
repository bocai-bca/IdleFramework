using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Threading;

namespace IdleFramework.Core;

/// <summary>
/// 存档数据辅助器，是对<c>SaveData</c>的操作工具。提供针对单一存档中部分数据为单位的API，必须搭配游戏资源使用。
/// 除非有明确说明，否则默认本类型提供的方法是线程安全的，每个涉及存档数据的操作都会包含锁，如果想要避免因互斥锁阻塞导致的性能下降，可以考虑复制<c>SaveData</c>然后通过它创建新的<c>SaveDataHelper</c>实例。
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
	[Pure]
	public long GetLastUpdateUtcTick()
	{
		lock (_lock)
		{
			return UsingSaveData.LastUpdateUtcTick;
		}
	}

	/// <summary>
	/// 通过GUID获取一个物品实例的名称，未找到时返回给定的默认名称。
	/// </summary>
	/// <param name="guid">要查找的实例的GUID。</param>
	/// <param name="defaultName">未找到时要返回的默认名称。</param>
	/// <returns>该物品实例的名称。</returns>
	[Pure]
	public string GetNameForInstance(Guid guid, string defaultName = "")
	{
		lock (_lock)
		{
			return UsingSaveData.InstanceNames.GetValueOrDefault(guid, defaultName);
		}
	}

	/// <summary>
	/// 设置对应给定GUID的物品实例的名称，如果名称表中原本不存在对应GUID将创建。
	/// </summary>
	/// <param name="guid">要设置名称的GUID。</param>
	/// <param name="name">要设置的名称。</param>
	public void SetNameForInstance(Guid guid, string name)
	{
		lock (_lock)
		{
			UsingSaveData.InstanceNames[guid] = name;
		}
	}

	/// <summary>
	/// 获取给定ID的空间的空间容器的GUID。
	/// </summary>
	/// <param name="spaceId">想要获取其空间容器的空间的ID。</param>
	/// <param name="spaceContainerGuid">获取到的空间容器GUID，如果未找到则返回<c>Guid.Empty</c>。</param>
	/// <returns>成功与否。</returns>
	public bool GetSpaceContainerGuidForSpace(string spaceId, out Guid spaceContainerGuid)
	{
		lock (_lock)
		{
			if (UsingSaveData.SpaceDatas.TryGetValue(spaceId, out SpaceData spaceData))
			{
				spaceContainerGuid = spaceData.SpaceContainerGuid;
				return true;
			}
		}
		spaceContainerGuid = Guid.Empty;
		return false;
	}

	/// <summary>
	/// 获取给定ID的空间的所有物品实例的GUID。本方法相当于获取<c>SpaceData.InstanceItemGuids</c>字典的复制品。
	/// </summary>
	/// <param name="spaceId">想要获取其空间容器的空间的ID。</param>
	/// <param name="instanceItemGuids">获取到的所有物品实例的GUID，键为物品ID，值为GUID列表，如果未找到则返回空字典。</param>
	/// <returns>成功与否。</returns>
	public bool GetAllInstanceGuidsInSpace(string spaceId, out Dictionary<string, List<Guid>> instanceItemGuids)
	{
		instanceItemGuids = [];
		lock (_lock)
		{
			if (!UsingSaveData.SpaceDatas.TryGetValue(spaceId, out SpaceData spaceData)) return false;
			foreach ((string itemId, List<Guid> instanceGuid) in spaceData.InstanceItemGuids)
			{
				instanceItemGuids[itemId] = new List<Guid>(instanceGuid);
			}
		}
		return true;
	}
	
	
	
	/// <summary>
	/// 通过GUID获取一个容器实例，可以选择是否要获取复制品。
	/// </summary>
	/// <param name="guid">要查找的GUID。</param>
	/// <param name="containerData">查找到的容器实例。</param>
	/// <param name="duplicate">是否要复制获取到的容器实例。</param>
	/// <returns>成功与否，如果没有找到则返回<c>false</c>。</returns>
	public bool GetContainerForGuid(Guid guid, [MaybeNullWhen(false)] out ContainerData containerData, bool duplicate = true)
	{
		lock (_lock)
		{
			if (!UsingSaveData.ContainerDatas.TryGetValue(guid, out containerData)) return false;
			containerData = duplicate ? containerData.Duplicate() : containerData;
		}
		return true;
	}

	/// <summary>
	/// 获取给定GUID的容器中的所有物品数量。本方法相当于获取<c>ContainerData.ItemCounts</c>字典的复制品。
	/// 方法的时间复杂度为O(n)。
	/// </summary>
	/// <param name="containerGuid">要查找的容器GUID。</param>
	/// <param name="containerItems">结果字典，键为物品ID，值为对应物品的物品数量。如果未能找到给定GUID的容器实例则返回空字典。</param>
	/// <returns>是否成功获取到对应容器，如果不存在对应GUID的容器则返回<c>false</c>。</returns>
	[Pure]
	public bool GetAllItemCountsForContainer(Guid containerGuid, out Dictionary<string, long> containerItems)
	{
		ContainerData containerData;
		lock (_lock)
		{
			if (!UsingSaveData.ContainerDatas.TryGetValue(containerGuid, out containerData))
			{
				containerItems = [];
				return false;
			}
			containerData = containerData.Duplicate();
		}
		containerItems = GetAllItemCountsForContainer(containerData);
		return true;
	}

	/// <summary>
	/// 获取给定GUID的容器中的所有物品数量。
	/// 方法的时间复杂度为O(n)。
	/// 注意，本方法非线程安全，请根据需要手动传入复制品<c>ContainerData</c>。
	/// </summary>
	/// <param name="containerData">要获取数据的容器。</param>
	/// <returns>结果字典，键为物品ID，值为对应物品的物品数量。</returns>
	[Pure]
	public static Dictionary<string, long> GetAllItemCountsForContainer([DisallowNull] ContainerData containerData)
	{
		Dictionary<string, long> result = [];
		foreach ((string itemId, long itemCount) in containerData.ItemCounts) result[itemId] = itemCount;
		return result;
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
	public static ContainerData InstantiateRegistryObject(ContainerRegistryObject containerRegistryObject)
	{
		return new ContainerData();
	}

	/// <summary>
	/// 从注册表实例化实例数据。
	/// </summary>
	/// <param name="factoryRegistryObject">想要实例化的工厂注册表项。</param>
	/// <returns>实例化的新工厂数据。</returns>
	public static FactoryData InstantiateRegistryObject(FactoryRegistryObject factoryRegistryObject)
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
			foreach ((string itemId, long itemCount) in spaceRegistryObject.PrefillItems) //设置预装物品
			{
				newSpaceContainerData.ItemCounts[itemId] = itemCount; //设置物品数量
				bool hasContainerRegistry = UsingGameResource.ContainerRegistry.TryGetValue(itemId, out ContainerRegistryObject prefillItemContainerRegistryObject);
				bool hasFactoryRegistry = UsingGameResource.FactoryRegistry.TryGetValue(itemId, out FactoryRegistryObject prefillItemFactoryRegistryObject);
				if (!hasContainerRegistry && !hasFactoryRegistry) continue;
				List<Guid> prefillInstanceGuids = [];
				for (long i = 0; i < itemCount; i++)
				{
					Guid currentInstanceGuid = Guid.NewGuid();
					if (hasContainerRegistry) AddInstanceObject(InstantiateRegistryObject(prefillItemContainerRegistryObject), currentInstanceGuid);
					if (hasFactoryRegistry) AddInstanceObject(InstantiateRegistryObject(prefillItemFactoryRegistryObject), currentInstanceGuid);
					prefillInstanceGuids.Add(currentInstanceGuid);
				}
				newSpaceData.InstanceItemGuids[itemId] = prefillInstanceGuids;
			}
			SetNameForInstance(newSpaceData.SpaceContainerGuid = AddInstanceObject(newSpaceContainerData), Localization.Tr("space_container"));
			AddSpaceData(spaceId, newSpaceData);
		}
	}

	public void AddSpaceData(string spaceId, SpaceData spaceData)
	{
		lock (_lock)
		{
			UsingSaveData.SpaceDatas[spaceId] = spaceData;
		}
	}
	
	/// <summary>
	/// 保护线程安全用的互斥锁，使用时应尽量缩小原子单元。
	/// </summary>
	private readonly Lock _lock = new();
}
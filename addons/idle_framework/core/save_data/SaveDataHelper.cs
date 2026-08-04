using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace IdleFramework.Core;

/// <summary>
/// 存档数据辅助器，是对<c>SaveData</c>的操作工具。提供针对单一存档中部分数据为单位的API，必须搭配游戏资源使用。
/// 除非有明确说明，否则默认本类型提供的方法是线程安全的，每个涉及存档数据的操作都会包含锁，如果想要避免因互斥锁阻塞导致的性能下降，可以考虑复制<c>SaveData</c>然后通过它创建新的<c>SaveDataHelper</c>实例。
/// 请避免在该<c>SaveData</c>不再可用时访问通过它创建的本类实例的方法，否则将发生空引用异常。
/// </summary>
public class SaveDataHelper(GameResource targetGameResource, SaveData targetSaveData)
{
	#region 使用的数据的成员 Using Data Members
	
	/// <summary>
	/// 目标游戏资源。注意，直接获取本属性时，其提供的值本身不是线程安全的。
	/// </summary>
	public GameResource UsingGameResource { get; } = targetGameResource;
	
	/// <summary>
	/// 目标存档数据。注意，直接获取本属性时，其提供的值本身不是线程安全的。
	/// </summary>
	public SaveData UsingSaveData { get; } = targetSaveData;

	#endregion
	
	#region 元数据访问 Meta Accessing
	
	/// <summary>
	/// 获取存档数据的上次更新的UTC时间刻。
	/// </summary>
	/// <returns>存档数据的<c>LastUpdateUtcTick</c>属性。</returns>
	public long GetLastUpdateUtcTick()
	{
		lock (_lock) return UsingSaveData.LastUpdateUtcTick;
	}

	/// <summary>
	/// 设置存档数据的上次更新的UTC时间刻。
	/// </summary>
	/// <param name="tick">要设置进存档数据的<c>LastUpdateUtcTick</c>属性的值。</param>
	public void SetLastUpdateUtcTick(long tick)
	{
		lock (_lock) UsingSaveData.LastUpdateUtcTick = tick;
	}
	
	#endregion

	#region 实例对象访问 InstanceObject Accessing
	
	#region 物品查询 Item Querying
	
	/// <summary>
	/// GUID到原始物品ID的缓存表。
	/// 此表不应在单个运行时生命周期中重置，因为GUID在理论上不应被重用，当一个GUID已确认被用于什么物品时，它没道理在后续变更为其他物品的实例对象的GUID。
	/// </summary>
	public Dictionary<Guid, string> GuidToItemIdCache { get; } = [];

	/// <summary>
	/// 非互斥锁方法。通过GUID寻找拥有该GUID的实例对象所属于的物品ID。
	/// </summary>
	/// <param name="guid">要查找的GUID。</param>
	/// <returns>该GUID的所有者所属于的物品ID，如果未找到则返回空字符串。</returns>
	public string QueryItemIdForGuid(Guid guid)
	{
		if (GuidToItemIdCache.TryGetValue(guid, out string result)) return result;
		foreach (string spaceId in UsingGameResource.SpaceRegistry.Keys)
		{
			if (!GetAllInstanceGuidsOfItemsInSpace(spaceId, out Dictionary<string, HashSet<Guid>> instanceItemGuids)) continue;
			foreach ((string itemId, HashSet<Guid> hashSet) in instanceItemGuids)
			{
				if (!hashSet.Contains(guid)) continue;
				GuidToItemIdCache.Add(guid, itemId);
				return itemId;
			}
		}
		return string.Empty;
	}

	/// <summary>
	/// 非互斥锁方法。通过GUID寻找拥有该GUID的实例对象所属于的物品ID。
	/// </summary>
	/// <param name="guid">要查找的GUID。</param>
	/// <param name="itemId">该GUID的所有者所属于的物品ID，如果未找到则为空字符串。</param>
	/// <returns>是否成功找到给定GUID的物品ID。</returns>
	public bool QueryItemIdForGuid(Guid guid, out string itemId)
	{
		if (GuidToItemIdCache.TryGetValue(guid, out itemId)) return true;
		foreach (string spaceId in UsingGameResource.SpaceRegistry.Keys)
		{
			if (!GetAllInstanceGuidsOfItemsInSpace(spaceId, out Dictionary<string, HashSet<Guid>> instanceItemGuids)) continue;
			foreach ((string instanceItemId, HashSet<Guid> hashSet) in instanceItemGuids)
			{
				if (!hashSet.Contains(guid)) continue;
				GuidToItemIdCache.Add(guid, instanceItemId);
				itemId = instanceItemId;
				return true;
			}
		}
		itemId = null;
		return false;
	}
	
	#endregion
	
	#region 名称存取 Name GetAndSet
	
	/// <summary>
	/// 通过GUID获取一个实例对象的名称，未找到时返回给定的默认名称。
	/// </summary>
	/// <param name="guid">要查找的实例对象的GUID。</param>
	/// <param name="defaultName">未找到时要返回的默认名称。</param>
	/// <returns>该实例对象的名称。</returns>
	public string GetNameForInstance(Guid guid, string defaultName = "")
	{
		lock (_lock) return UsingSaveData.InstanceNames.GetValueOrDefault(guid, defaultName);
	}

	/// <summary>
	/// 设置对应给定GUID的实例对象的名称，如果名称表中原本不存在对应GUID将创建。
	/// </summary>
	/// <param name="guid">要设置名称的实例对象的GUID。</param>
	/// <param name="name">要设置的名称。</param>
	public void SetNameForInstance(Guid guid, string name)
	{
		lock (_lock) UsingSaveData.InstanceNames[guid] = name;
	}
	
	#endregion
	
	#region 数据实例操作 Data Instance Operating
	
	/// <summary>
	/// 将给定实例对象添加到存档数据并返回GUID，如果不指定自定义的GUID将随机创建新的GUID。如果已有相同GUID的容器数据则会进行覆盖。
	/// </summary>
	/// <param name="containerData">要添加的容器数据。</param>
	/// <param name="guid">要使用的GUID，可以使用<c>Guid.NewGuid()</c>创建新GUID，或者不使用此项参数(保持此项参数的值为default)来自动创建新的GUID。</param>
	/// <returns>该实例物品被添加为的GUID，在不指定自定义GUID的情况下返回随机的新GUID，否则返回给定的自定义GUID。</returns>
	public Guid SetInstanceObject(ContainerData containerData, Guid guid = default)
	{
		if (guid == Guid.Empty) guid = Guid.NewGuid();
		lock (_lock) UsingSaveData.ContainerDatas[guid] = containerData;
		return guid;
	}
	
	/// <summary>
	/// 将给定实例对象添加到存档数据并返回GUID，如果不指定自定义的GUID将随机创建新的GUID。如果已有相同GUID的工厂数据则会进行覆盖。
	/// </summary>
	/// <param name="factoryData">要添加的工厂数据。</param>
	/// <param name="guid">要使用的GUID，可以使用<c>Guid.NewGuid()</c>创建新GUID，或者不使用此项参数(保持此项参数的值为default)来自动创建新的GUID。</param>
	/// <returns>该实例物品被添加为的GUID，在不指定自定义GUID的情况下返回随机的新GUID，否则返回给定的自定义GUID。</returns>
	public Guid SetInstanceObject(FactoryData factoryData, Guid guid = default)
	{
		if (guid == Guid.Empty) guid = Guid.NewGuid();
		lock (_lock) UsingSaveData.FactoryDatas[guid] = factoryData;
		return guid;
	}

	/// <summary>
	/// 将给定实例对象添加到存档数据并返回GUID，如果不指定自定义的GUID将随机创建新的GUID。如果已有相同GUID的富数据物品数据则会进行覆盖。
	/// </summary>
	/// <param name="richDataItemData">要添加的富数据物品数据。</param>
	/// <param name="guid">要使用的GUID，可以使用<c>Guid.NewGuid()</c>创建新GUID，或者不使用此项参数(保持此项参数的值为default)来自动创建新的GUID。</param>
	/// <returns>该实例物品被添加为的GUID，在不指定自定义GUID的情况下返回随机的新GUID，否则返回给定的自定义GUID。</returns>
	public Guid SetInstanceObject(RichDataItemData richDataItemData, Guid guid = default)
	{
		if (guid == Guid.Empty) guid = Guid.NewGuid();
		lock (_lock) UsingSaveData.RichDataItems[guid] = richDataItemData;
		return guid;
	}

	#endregion
	
	#endregion

	#region 空间访问 Space Accessing

	/// <summary>
	/// 获取所有空间数据的ID。
	/// 此方法暂时不推荐使用，因为可以直接通过不使用锁地访问游戏数据来获得。
	/// </summary>
	/// <returns>所有空间数据的ID。</returns>
	public ICollection<string> GetAllSpaceIds()
	{
		lock (_lock)
		{
			return new List<string>(UsingSaveData.SpaceDatas.Keys);
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
	/// <returns>成功与否，一般不太可能为<c>false</c>因为空间数据会在存档数据初始化时实例化所有空间注册表项。</returns>
	public bool GetAllInstanceGuidsOfItemsInSpace(string spaceId, out Dictionary<string, HashSet<Guid>> instanceItemGuids)
	{
		instanceItemGuids = [];
		lock (_lock)
		{
			if (!UsingSaveData.SpaceDatas.TryGetValue(spaceId, out SpaceData spaceData)) return false;
			foreach ((string itemId, HashSet<Guid> instanceGuid) in spaceData.InstanceItemGuids)
			{
				instanceItemGuids[itemId] = new HashSet<Guid>(instanceGuid);
			}
		}
		return true;
	}
	
	#endregion
	
	#region 容器访问 Container Accessing
	
	#region 混合 Mixin

	/// <summary>
	/// 查找是否存在对应给定GUID的容器实例或容器编组。
	/// </summary>
	/// <param name="guid">要查找的GUID。</param>
	/// <returns>是否存在对应GUID的容器实例或容器编组。</returns>
	public bool IsContainerMixinExistsForGuid(Guid guid)
	{
		lock (_lock)
		{
			return UsingSaveData.ContainerDatas.ContainsKey(guid) || UsingSaveData.ContainerGroups.ContainsKey(guid);
		}
	}
	
	/// <summary>
	/// 获取给定GUID的容器或容器编组中的所有物品数量。本方法相当于获取特定单一容器或一个容器编组中的所有嵌套容器的<c>ContainerData.ItemCounts</c>字典的复制品。
	/// 方法的时间复杂度为O(n)。
	/// 如果同时存在相同GUID的容器实例和容器编组，将只返回容器实例的物品，因为理论上不允许有相同GUID的容器实例和容器编组存在。
	/// 如果查找到的容器编组存在嵌套，则会递归进去进一步搜索物品。
	/// </summary>
	/// <param name="containerMixinGuid">要查找的容器实例或容器编组的GUID。</param>
	/// <param name="containerItems">结果字典，键为物品ID，值为对应物品的物品数量。如果未能找到给定GUID的容器实例或容器编组则返回空字典。</param>
	/// <returns>是否成功获取到对应容器实例或容器编组，如果不存在对应GUID的容器实例或容器编组则返回<c>false</c>。</returns>
	public bool GetAllItemCountsForContainerMixin(Guid containerMixinGuid, out Dictionary<string, long> containerItems)
	{
		bool gotContainerData;
		bool gotContainerGroup;
		ContainerData containerData;
		ContainerGroup containerGroup;
		//获取容器实例或容器编组，并复制
		lock (_lock)
		{
			gotContainerData = UsingSaveData.ContainerDatas.TryGetValue(containerMixinGuid, out containerData);
			if (gotContainerData) containerData = containerData.Duplicate();
			gotContainerGroup = UsingSaveData.ContainerGroups.TryGetValue(containerMixinGuid, out containerGroup);
			if (gotContainerGroup) containerGroup = containerGroup.Duplicate();
		}
		if (gotContainerData) //如果找到了容器实例
		{
			//从容器中获取所有物品并返回
			containerItems = GetAllItemCountsForContainer(containerData);
			return true;
		}
		containerItems = [];
		if (!gotContainerGroup) return false; //如果没有找到容器实例或容器编组，在这里返回为失败状态
		foreach (Guid guidContainerMixin in containerGroup.Guids) //遍历取得的容器编组的所有GUID
		{
			if (!GetAllItemCountsForContainerMixin(guidContainerMixin, out Dictionary<string, long> deepContainerItems)) continue; //如果当前GUID没有查询到容器实例或容器编组则进入下一个GUID
			foreach ((string deepItemId, long deepItemCount) in deepContainerItems) //遍历取得的深层容器物品
			{
				containerItems[deepItemId] = deepItemCount + containerItems.GetValueOrDefault(deepItemId, 0); //追加当前深层物品的数量进浅层容器物品表
			}
		}
		return true;
	}

	/// <summary>
	/// 尝试从给定GUID的容器或容器编组中消耗物品，并返回是否成功消耗要求的物品，如果给定的容器或容器编组不满足要求的物品数量则不会消耗物品。
	/// 方法的时间复杂度为O(n)，n代表给定GUID可探索到的容器或容器编组的总数。
	/// 如果查找到的容器编组存在嵌套，则会递归进去进一步搜索物品。
	/// </summary>
	/// <param name="containerMixinGuid">要遍历的容器实例或容器编组的GUID。</param>
	/// <param name="itemCounts">要消耗的物品的数量，键为物品ID，值为物品数量，0或负数的物品数量不会消耗对应物品，但会要求该物品可在遍历的容器实例或容器编组中存在(库存数量大于0)。</param>
	/// <returns>是否成功消耗要求的物品数量。</returns>
	public bool TryConsumeItemCountsForContainerMixin(Guid containerMixinGuid, Dictionary<string, long> itemCounts)
	{
		//注意要避免在lock中使用辅助器方法。
		//检查物品是否满足
		
		//尝试消耗物品
		return true;
	}
	
	#endregion
	
	#region 仅容器 Container Only
	
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
	public static Dictionary<string, long> GetAllItemCountsForContainer([DisallowNull] ContainerData containerData)
	{
		Dictionary<string, long> result = [];
		foreach ((string itemId, long itemCount) in containerData.ItemCounts) result[itemId] = itemCount;
		return result;
	}
	
	/// <summary>
	/// 尝试从给定GUID的容器中消耗物品，并返回是否成功消耗要求的物品，如果给定的容器不满足要求的物品数量则不会消耗物品。
	/// 方法的时间复杂度近似O(n)，n代表要求消耗的物品类型的总数。
	/// </summary>
	/// <param name="containerGuid">要消耗物品的容器实例的GUID。</param>
	/// <param name="itemCountsForConsume">要消耗的物品的数量，键为物品ID，值为物品数量，0或负数的物品数量不会消耗对应物品，但会要求该物品可在遍历的容器实例中存在(库存数量大于0)。</param>
	/// <returns>是否成功消耗要求的物品数量。</returns>
	public bool TryConsumeItemsForContainer(Guid containerGuid, Dictionary<string, long> itemCountsForConsume)
	{
		lock (_lock)
		{
			if (!UsingSaveData.ContainerDatas.TryGetValue(containerGuid, out ContainerData containerData)) return false;
			//检查物品是否满足
			foreach ((string requiredItemId, long requiredItemCount) in itemCountsForConsume)
			{
				if (!containerData.ItemCounts.TryGetValue(requiredItemId, out long haveItemCount)) return false;
				if (haveItemCount < requiredItemCount) return false;
			}
			//消耗物品
			foreach ((string requiredItemId, long requiredItemCount) in containerData.ItemCounts) containerData.ItemCounts[requiredItemId] -= Math.Clamp(requiredItemCount, 0L, long.MaxValue);
		}
		return true;
	}

	/// <summary>
	/// 尝试向给定GUID的容器中添加物品，并返回是否成功添加给定的物品，如果给定的容器物品数量溢出也将成功添加物品。
	/// 方法的时间复杂度近似O(n)，n代表要求添加的物品类型的总数。
	/// </summary>
	/// <param name="containerGuid">要添加物品的容器实例的GUID。</param>
	/// <param name="itemCountsForAdd">要添加的物品的数量，键为物品ID，值为物品数量，0或负数的物品数量不会执行操作。</param>
	/// <returns>是否成功添加给定的物品。在找不到符合给定GUID的容器实例或无法获取该容器实例所属的物品ID或无法在注册表中寻找到物品ID的注册表项时会返回<c>false</c>。</returns>
	public bool TryAddItemsForContainer(Guid containerGuid, Dictionary<string, long> itemCountsForAdd)
	{
		lock (_lock)
		{
			if (!UsingSaveData.ContainerDatas.TryGetValue(containerGuid, out ContainerData containerData)) return false;
			if (!QueryItemIdForGuid(containerGuid, out string itemId)) return false;
			if (!UsingGameResource.ContainerRegistry.TryGetValue(itemId, out ContainerRegistryObject containerRegistryObject)) return false;
			foreach ((string addItemId, long addItemCount) in itemCountsForAdd)
			{
				long maxStackThisItem = containerRegistryObject.ItemMaxStacks.GetCountForItem(UsingGameResource.ItemRegistry, addItemId);
				long maxAddThisItem = maxStackThisItem - maxStackThisItem;
				containerData.ItemCounts[addItemId] += Math.Min(addItemCount, maxAddThisItem);
			}
		}
		return true;
	}
	
	#endregion
	
	#region 仅容器编组 ContainerGroup Only
	
	/// <summary>
	/// 通过GUID获取一个容器编组，可以选择是否要获取复制品。
	/// </summary>
	/// <param name="guid">要查找的GUID。</param>
	/// <param name="containerGroup">查找到的容器编组。</param>
	/// <param name="duplicate">是否要复制获取到的容器编组。</param>
	/// <returns>成功与否，如果没有找到则返回<c>false</c>。</returns>
	public bool GetContainerGroupForGuid(Guid guid, [MaybeNullWhen(false)] out ContainerGroup containerGroup, bool duplicate = true)
	{
		lock (_lock)
		{
			if (!UsingSaveData.ContainerGroups.TryGetValue(guid, out containerGroup)) return false;
			containerGroup = duplicate ? containerGroup.Duplicate() : containerGroup;
		}
		return true;
	}
	
	#endregion
	
	#endregion
	
	#region 工厂访问 Factory Accessing
	
	/// <summary>
	/// 通过GUID获取一个工厂实例，可以选择是否要获取复制品。
	/// </summary>
	/// <param name="guid">要查找的GUID。</param>
	/// <param name="factoryData">查找到的工厂实例。</param>
	/// <param name="duplicate">是否要复制获取到的工厂实例。</param>
	/// <returns>成功与否，如果没有找到则返回<c>false</c>。</returns>
	public bool GetFactoryForGuid(Guid guid, [MaybeNullWhen(false)] out FactoryData factoryData, bool duplicate = true)
	{
		lock (_lock)
		{
			if (!UsingSaveData.FactoryDatas.TryGetValue(guid, out factoryData)) return false;
			factoryData = duplicate ? factoryData.Duplicate() : factoryData;
		}
		return true;
	}

	/// <summary>
	/// 获取所有工厂的GUID集合。
	/// </summary>
	/// <returns>一个容纳当前所有工厂GUID的集合。</returns>
	public ICollection<Guid> GetAllGuidsForFactories()
	{
		lock (_lock)
		{
			return new List<Guid>(UsingSaveData.FactoryDatas.Keys);
		}
	}
	
	#endregion
	
	#region 注册表项实例化 RegistryObject Instantiating
	
	/// <summary>
	/// 从注册表实例化实例物品数据。
	/// </summary>
	/// <param name="containerRegistryObject">想要实例化的容器注册表项。</param>
	/// <returns>实例化的新容器数据。</returns>
	public static ContainerData InstantiateRegistryObject(ContainerRegistryObject containerRegistryObject)
	{
		return new ContainerData();
	}

	/// <summary>
	/// 从注册表实例化实例物品数据。
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
	
	#endregion
	
	#region 存档初始化 SaveData Initializing
	
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
	
	#endregion
	
	#region 空间操作 Space Operating
	
	/// <summary>
	/// 初始化空间，通过游戏资源的空间注册表为存档数据的空间表创建空间数据实例，同时创建空间容器，并添加到容器表。
	/// </summary>
	public void InitSpaces()
	{
		foreach ((string spaceId, SpaceRegistryObject spaceRegistryObject) in UsingGameResource.SpaceRegistry)
		{
			SpaceData newSpaceData = new(); //该空间的空间数据
			ContainerData newSpaceContainerData = new(); //新空间的空间容器的容器数据
			foreach ((string itemId, long itemCount) in spaceRegistryObject.PrefillItems) //设置预装物品
			{
				newSpaceContainerData.ItemCounts[itemId] = itemCount; //设置物品数量
				bool hasContainerRegistry = UsingGameResource.ContainerRegistry.TryGetValue(itemId, out ContainerRegistryObject prefillItemContainerRegistryObject);
				bool hasFactoryRegistry = UsingGameResource.FactoryRegistry.TryGetValue(itemId, out FactoryRegistryObject prefillItemFactoryRegistryObject);
				if (!hasContainerRegistry && !hasFactoryRegistry) continue; //从此往下可确保该物品ID至少有在容器注册表或工厂注册表中被注册成为了一种
				HashSet<Guid> prefillInstanceGuids = []; //空间容器的预装物品实例GUID表
				for (long i = 0; i < itemCount; i++) //遍历物品数量次，创建复数个实例物品实例
				{
					Guid currentInstanceGuid = Guid.NewGuid(); //创建GUID，待会儿如果容器和工厂同时有，好让它们拥有同一个GUID(而且这是必须的)
					SetNameForInstance(currentInstanceGuid, UsingGameResource.GetItemNameTranslated(itemId) + "#" + currentInstanceGuid.ToString()[^4..]);
					if (hasContainerRegistry) //检查是否被注册为容器
					{
						//创建容器实例并添加的过程
						SetInstanceObject(InstantiateRegistryObject(prefillItemContainerRegistryObject), currentInstanceGuid);
					}
					if (hasFactoryRegistry) //检查是否被注册为工厂
					{
						//创建工厂实例并添加的过程
						SetInstanceObject(InstantiateRegistryObject(prefillItemFactoryRegistryObject), currentInstanceGuid);
					}
					prefillInstanceGuids.Add(currentInstanceGuid);
				}
				newSpaceData.InstanceItemGuids[itemId] = prefillInstanceGuids;
			}
			SetNameForInstance(newSpaceData.SpaceContainerGuid = SetInstanceObject(newSpaceContainerData), Localization.Tr("space_container")); //给空间容器实例设置名字
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
	
	#endregion
	
	/// <summary>
	/// 保护线程安全用的互斥锁，使用时应尽量缩小原子单元。
	/// </summary>
	private readonly Lock _lock = new();
}
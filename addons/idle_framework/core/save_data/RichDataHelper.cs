using System;
using System.Collections.Generic;
using Godot;

namespace IdleFramework.Core;

/// <summary>
/// 富数据物品数据(RDI)帮助类。
/// </summary>
public static class RichDataHelper
{
	/// <summary>
	/// 富数据键常量-容器。叶子路径为<c>Container</c>。
	/// </summary>
	public const string RDIKey_Container = "Container";

	/// <summary>
	/// 富数据键常量-工厂当前运行中的配方。叶子路径为<c>FactoryCurrentRecipe</c>。
	/// </summary>
	public const string RDIKey_FactoryCurrentRecipe = "FactoryCurrentRecipe";
	
	/// <summary>
	/// 富数据键常量-富数据物品GUID表。叶子路径为<c>Container.Guids</c>。
	/// </summary>
	public const string RDIKey_RDIGuids = "Guids";
	
	/// <summary>
	/// 富数据键常量-物品数量。叶子路径为<c>Container.&lt;ItemID&gt;.Count</c>。
	/// </summary>
	public const string RDIKey_ItemCount = "Count";
	
	extension(RichDataItemData rdi)
	{
		/// <summary>
		/// 本方法应调用于富数据物品的根数据上。
		/// 使该富数据物品数据实例拥有作为容器的相关数据，如果已有将不会覆盖。然后返回该容器富数据。
		/// 相当于设置该RDI的Container并直接获取。
		/// </summary>
		public RichDataItemData PlaceContainer()
		{
			// rdi = ./
			if (rdi.GetData(RDIKey_Container, out RichDataItemData container)) return container;
			container = new(); //一个RichDataItemData，其内部用作字典，存储该容器的所有物品，键为物品ID，值为存储物品的数量和富数据GUID的表
			rdi.SetData(RDIKey_Container, container);
			return container;
		}

		/// <summary>
		/// 本方法应调用于富数据物品的根数据上。
		/// 从该RDI实例获取其容器。
		/// 相当于获取该RDI的Container。
		/// </summary>
		/// <param name="container">将被获取到的容器列表。</param>
		/// <returns>是否成功获取，如果该RDI实例不存在<c>RDIKey_Container</c>键则获取失败。</returns>
		public bool TryGetContainer(out RichDataItemData container)
		{
			// rdi = ./
			return rdi.GetData(RDIKey_Container, out container);
		}

		/// <summary>
		/// 本方法应调用于富数据物品的容器数据上(Container)，关于容器另见<c>PlaceContainer()</c>和<c>TryGetContainer()</c>。
		/// 向容器RDI添加给定数量的给定物品。
		/// 相当于在该RDI添加对象 物品ID: { 数量: long }，或在现有基础上修改。
		/// </summary>
		/// <param name="itemId">要添加的物品的ID。</param>
		/// <param name="count">要添加的数量。</param>
		/// <param name="maxStackCount">允许该物品的最大堆叠数量，该参数应当在上游代码自行访问游戏资源获取。</param>
		/// <returns>添加后的新数量。</returns>
		/// <remarks>如果容器中存在与物品ID同名的键值对但值类型不同，也会对值进行覆盖。</remarks>
		public long AddItem(string itemId, long count, long maxStackCount)
		{
			// rdi = ./Container
			long itemCount;
			if (rdi.GetData(itemId, out RichDataItemData itemData)) //如果容器中有对应物品，获取对应物品的物品数据
			{
				itemData.GetData(RDIKey_ItemCount, out itemCount); //从对应物品的物品数据获取数量，存储到itemCount局部变量
			}
			else //否则(容器中不存在对应物品)
			{
				itemCount = 0; //设置物品数量为0
				itemData = new(); //新建物品数据
				rdi.SetData(itemId, itemData); //将新建的物品数据存储到容器，键为给定物品ID
			}
			// itemData = ./Container/{ItemID}
			// itemCount = ./Container/{ItemID}/Count
			long resultCount = itemCount + Math.Clamp(count, 0L, maxStackCount - itemCount); //声明局部变量并计算结果数量
			itemData.SetData(RDIKey_ItemCount, resultCount); //在物品数据中设置物品数量
			return resultCount; //返回物品数量
		}

		/// <summary>
		/// 创建新RDI实例并添加到存档的RDI字典中，同时返回该新实例及其GUID。
		/// 随后也可通过从RDIs字典使用本GUID索引来获得该新建的RDI实例。
		/// </summary>
		/// <param name="saveDataRDIs">要添加新实例的存档数据中的RDI字典。</param>
		/// <param name="newRid">新建的RDI实例。</param>
		/// <returns>新实例的GUID。</returns>
		public static Guid CreateRichDataItemInstance(Dictionary<Guid, RichDataItemData> saveDataRDIs, out RichDataItemData newRid)
		{
			if (saveDataRDIs == null)
			{
				Logger.LogError(Localization.Tr("log.error.rich_data_helper.save_data_rdi_dictionary_givened_is_null"));
				newRid = null;
				return Guid.Empty;
			}
			newRid = new();
			Guid newGuid = Guid.NewGuid();
			saveDataRDIs[newGuid] = newRid;
			return newGuid;
		}
	}
}
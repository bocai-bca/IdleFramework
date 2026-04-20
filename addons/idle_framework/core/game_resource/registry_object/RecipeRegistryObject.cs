using Godot;
using Godot.Collections;

namespace IdleFramework;

/// <summary>
/// 配方注册表对象，用于在游戏资源中注册配方
/// </summary>
[GlobalClass]
public partial class RecipeRegistryObject : Resource
{
	/// <summary>
	/// 该配方的原材料表。
	/// 如果原材料物品的数值提供器返回结果是会变化的，生产器会在配方订单创建时确定需求数量，在满足该需求数量的情况下才进行生产计时，在计时完毕后再次访问数值提供器取得需求数量，并扣除此时得到的需求数量个原料，而如果原料库存量未能满足此时的需求数量，该次生产将被重置到初始状态。
	/// </summary>
	[Export]
	[ExportGroup("Data")]
	public Dictionary<StringName, NumberProvider> Ingredients = new();
	
	/// <summary>
	/// 配方制作所需时间，单位为秒。
	/// 原材料满足时计时，计时满后扣除原材料并产出产品。
	/// 如果给定的数值提供器的返回结果是可变的，此次配方订单生产所需时间会在开始生产时从数值提供器获取，该时间会缓存在该配方订单数据中。
	/// </summary>
	[Export]
	public NumberProvider RequiredSeconds;
	
	/// <summary>
	/// 产品表。
	/// 如果产品配方物品的数值提供器返回的结果是可变的，没啥大碍，可变数值提供器就是为这种位置量身打造的。
	/// 键为物品ID，值为需要的物品数量
	/// </summary>
	[Export]
	public Dictionary<StringName, NumberProvider> Results = new();
}
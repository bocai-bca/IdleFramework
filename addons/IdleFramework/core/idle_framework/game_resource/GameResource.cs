using Godot;
using Godot.Collections;

namespace IdleFramework;

/// <summary>
/// 游戏资源
/// </summary>
[GlobalClass]
public partial class GameResource : Resource
{
	/// <summary>
	/// 物品注册表，在该列表中添加项即可注册物品
	/// </summary>
	[Export]
	public Dictionary<StringName, ItemRegistryObject> ItemRegistry { get; set; }
}
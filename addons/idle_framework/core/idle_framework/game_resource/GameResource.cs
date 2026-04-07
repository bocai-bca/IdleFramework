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
	/// 空间注册表，在该列表中添加项即可注册空间，字典键为其id
	/// </summary>
	[Export] public Dictionary<StringName, SpaceRegistryObject> SpaceRegistry { get; set; } = new();
	
	/// <summary>
	/// 物品注册表，在该列表中添加项即可注册物品，字典键为其id
	/// </summary>
	[Export] public Dictionary<StringName, ItemRegistryObject> ItemRegistry { get; set; } = new();
	
	/// <summary>
	/// 配方注册表，在该列表中添加项即可注册配方，字典键为其id
	/// </summary>
	[Export] public Dictionary<StringName, RecipeRegistryObject> RecipeRegistry { get; set; } = new();
	
	/// <summary>
	/// 容器注册表，在该列表中添加项即可注册容器，字典键为其id
	/// </summary>
	[Export] public Dictionary<StringName, ContainerRegistryObject> ContainerRegistry { get; set; } = new();
	
	/// <summary>
	/// 工厂注册表，在该列表中添加项即可注册工厂，字典键为其id
	/// </summary>
	[Export] public Dictionary<StringName, FactoryRegistryObject> FactoryRegistry { get; set; } = new();
}
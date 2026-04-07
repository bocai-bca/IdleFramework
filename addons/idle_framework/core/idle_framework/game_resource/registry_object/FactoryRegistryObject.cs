using Godot;

namespace IdleFramework;

/// <summary>
/// 工厂注册表对象，用于在游戏资源中注册工厂
/// </summary>
[GlobalClass]
public partial class FactoryRegistryObject : Resource
{
	/// <summary>
	/// 可使用的配方
	/// </summary>
	[Export] public RecipesProvider RecipesAbleToUse { get; set; }
	
	/// <summary>
	/// 配方下单器
	/// </summary>
	[Export] public RecipeOrder RecipeOrder { get; set; }
}
using Godot;
using IdleFramework.Core;

namespace IdleFramework;

/// <summary>
/// 工厂注册表对象，用于在游戏资源中注册工厂
/// </summary>
[GlobalClass]
public partial class FactoryRegistryObject : Resource
{
	/// <summary>
	/// 配方下单器
	/// </summary>
	[Export]
	[ExportGroup("Data")]
	public RecipeOrder RecipeOrder { get; set; }
	
	/// <summary>
	/// 工厂原材料需求模式
	/// </summary>
	[Export]
	public FactoryIngredientRequireMode IngredientRequireMode { get; set; }
}
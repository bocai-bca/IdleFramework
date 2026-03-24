using Godot;

namespace IdleFramework;

/// <summary>
/// 物品注册表对象，用于在游戏资源中注册物品
/// </summary>
[GlobalClass]
public partial class ItemRegistryObject: Resource
{
	/// <summary>
	/// 
	/// </summary>
	[Export] public StringName NameKey { get; set; }
	[Export] public StringName LoreKey { get; set; }
}
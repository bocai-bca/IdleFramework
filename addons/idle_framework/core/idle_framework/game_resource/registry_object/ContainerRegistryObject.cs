using Godot;

namespace IdleFramework;

/// <summary>
/// 容器注册表对象，用于在游戏资源中注册容器
/// </summary>
[GlobalClass]
public partial class ContainerRegistryObject : Resource
{
	/// <summary>
	/// 该空间容器的物品最大容量覆写
	/// </summary>
	[Export] public ItemMaxStacksProvider ItemMaxStacks { get; set; } = new();
}
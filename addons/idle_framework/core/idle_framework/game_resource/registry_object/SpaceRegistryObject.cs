using Godot;
using Godot.Collections;

namespace IdleFramework;

/// <summary>
/// 空间注册表对象，用于在游戏资源中注册空间
/// </summary>
[GlobalClass]
public partial class SpaceRegistryObject : Resource
{
	/// <summary>
	/// 空间名称翻译键
	/// </summary>
	[Export] public StringName NameKey { get; set; } = "";

	/// <summary>
	/// 空间描述翻译键
	/// </summary>
	[Export]public StringName LoreKey { get; set; } = "";

	/// <summary>
	/// 图标纹理图
	/// </summary>
	[Export] public Texture2D IconTexture { get; set; }
	
	/// <summary>
	/// 该空间容器的物品最大容量覆写
	/// </summary>
	[Export] public ItemMaxStacksProvider ItemMaxStacks { get; set; } = new();
	
	/// <summary>
	/// 预装物品，字典键为物品id，值为对应物品的数量
	/// </summary>
	[Export] public Dictionary<StringName, int> PrefillItems { get; set; } = new();
	
}
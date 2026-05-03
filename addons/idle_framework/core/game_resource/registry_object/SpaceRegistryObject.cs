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
	[Export]
	[ExportGroup("Assets")]
	public string NameKey { get; set; } = "";

	/// <summary>
	/// 空间描述翻译键
	/// </summary>
	[Export]
	public string LoreKey { get; set; } = "";

	/// <summary>
	/// 图标纹理图
	/// </summary>
	[Export]
	public Texture2D IconTexture { get; set; }
	
	/// <summary>
	/// 该空间容器的物品最大容量覆写
	/// </summary>
	[Export]
	[ExportGroup("Data")]
	public ItemMaxStacksProvider ItemMaxStacks { get; set; } = new();
	
	/// <summary>
	/// 预装物品，字典键为物品id，值为对应物品的数量。
	/// 如果预装物品给定的数量大于最大容量指定的数量，则初始化填充时会填充此处给定的数量，后续容器中对应物品数量如何变化取决于其他增减逻辑(包括自定义脚本所控制的逻辑)。
	/// </summary>
	[Export]
	public Dictionary<string, int> PrefillItems { get; set; } = new();
	
}
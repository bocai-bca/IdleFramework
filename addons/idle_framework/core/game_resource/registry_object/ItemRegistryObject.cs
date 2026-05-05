using Godot;

namespace IdleFramework;

/// <summary>
/// 物品注册表对象，用于在游戏资源中注册物品
/// </summary>
[GlobalClass]
public partial class ItemRegistryObject: Resource
{
	/// <summary>
	/// 物品名称翻译键
	/// </summary>
	[Export]
	[ExportGroup("Assets")]
	public string NameKey { get; set; } = "";

	/// <summary>
	/// 物品描述翻译键
	/// </summary>
	[Export]
	public string LoreKey { get; set; } = "";

	/// <summary>
	/// 图标纹理图
	/// </summary>
	[Export]
	public Texture2D IconTexture { get; set; }

	/// <summary>
	/// 默认在单个容器中的最大堆叠数量，默认为十亿。
	/// </summary>
	[Export]
	[ExportGroup("Data")]
	public long DefaultMaxStackCount { get; set; } = 1_000_000_000L;
	
}
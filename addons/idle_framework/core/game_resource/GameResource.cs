using Godot;
using Godot.Collections;

namespace IdleFramework.Core;

/// <summary>
/// 游戏资源。
/// 出于线程安全考虑，不要在运行时修改游戏资源以及其中嵌套的任何内容，游戏资源在设计上应当是只读的。
/// </summary>
[GlobalClass]
public partial class GameResource : Resource
{
	/// <summary>
	/// 本放置引擎游戏的GUID(全局唯一标识符)，主要用于在存档数据中记录其对应的游戏。
	/// 本ID会在文件系统中用作存储本游戏的存档数据的目录名称，所以请务必确保其不含文件命名非法字符，如<c>?</c>、<c>/</c>、<c>\</c>、<c>&lt;</c>、<c>&gt;</c>等字符。
	/// 格式方面建议点记法蛇形命名(<c>namespace.snake_case</c>)，推荐使用反向域名表示法，例如<c>net.bcasoft.idle_framework</c>。
	/// </summary>
	[Export, ExportGroup("Metadata")]
	public string GameID { get; set; } = "";

	/// <summary>
	/// 本放置引擎游戏的名称翻译键
	/// </summary>
	[Export]
	public string NameKey { get; set; } = "";
	
	/// <summary>
	/// 本放置引擎游戏的版本号
	/// </summary>
	[Export]
	public int GameVersion { get; set; } = 0;
	
	/// <summary>
	/// 空间注册表，在该列表中添加项即可注册空间，字典键为其id
	/// </summary>
	[Export, ExportGroup("Registries")]
	public Dictionary<string, SpaceRegistryObject> SpaceRegistry { get; set; } = new();
	
	/// <summary>
	/// 物品注册表，在该列表中添加项即可注册物品，字典键为其id
	/// </summary>
	[Export]
	public Dictionary<string, ItemRegistryObject> ItemRegistry { get; set; } = new();
	
	/// <summary>
	/// 配方注册表，在该列表中添加项即可注册配方，字典键为其id
	/// </summary>
	[Export]
	public Dictionary<string, RecipeRegistryObject> RecipeRegistry { get; set; } = new();
	
	/// <summary>
	/// 容器注册表，在该列表中添加项即可注册容器，字典键为其id
	/// </summary>
	[Export]
	public Dictionary<string, ContainerRegistryObject> ContainerRegistry { get; set; } = new();
	
	/// <summary>
	/// 工厂注册表，在该列表中添加项即可注册工厂，字典键为其id
	/// </summary>
	[Export]
	public Dictionary<string, FactoryRegistryObject> FactoryRegistry { get; set; } = new();
	
	/// <summary>
	/// 翻译数据
	/// </summary>
	[Export, ExportGroup("Data")]
	public Array<Translation> Translations { get; set; }
	
	/// <summary>
	/// 附加数据，供自定义UI场景或游戏资源中的自定义脚本访问。
	/// </summary>
	[Export]
	public Dictionary<string, Variant> AdditionData { get; set; } = new();
}
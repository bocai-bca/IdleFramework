using Godot;

namespace IdleFramework;

/// <summary>
/// 主节点，本脚本需附着在一个Node节点上，该节点作为项目的主场景根节点。也充当运行时数据管理器的功能，托管当前游戏实例上的重要数据，如游戏资源等。
/// </summary>
[GlobalClass]
public partial class MotherNode : Node
{
	/// <summary>
	/// 单例模式的实例
	/// </summary>
	public static MotherNode Instance
	{
		get
		{
			return _instance;
		}
		set
		{
			if (_instance != value) _instance?.QueueFree();
			_instance = value;
		}
	}
	
	/// <summary>
	/// UI场景，指定一个PackedScene作为用于呈现画面和处理玩家输入的场景，该场景需要自行访问核心来获取数据。
	/// 若不知道如何实现一个功能完备的自定义UI场景，建议使用原厂UI场景
	/// </summary>
	[Export]
	public PackedScene UIScene { get; set; }

	/// <summary>
	/// 游戏资源，指定一个IdleFramework.GameResource，作为定义游戏内容的数据源
	/// </summary>
	[Export]
	public GameResource GameResource { get; set; }

	public override void _EnterTree()
	{
		Instance = this;
	}

	public override void _Ready()
	{
	}


	public override void _Process(double delta)
	{
	}
	
	private static MotherNode _instance;
}
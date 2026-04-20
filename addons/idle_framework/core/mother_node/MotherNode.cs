using Godot;
using Godot.Collections;

namespace IdleFramework;

/// <summary>
/// 主节点，本脚本需附着在一个<c>Node</c>节点上，该节点作为项目的主场景根节点。也充当运行时数据管理器的功能，托管当前游戏实例上的重要数据，如游戏资源等。
/// </summary>
[GlobalClass]
public partial class MotherNode : Node
{

	/// <summary>
	/// 单例模式的实例
	/// 会在新的本类场景实例化后被覆盖
	/// </summary>
	public static MotherNode Instance
	{
		get;
		set
		{
			if (field != value) field?.QueueFree();
			field = value;
		}
	}

	/// <summary>
	/// UI场景打包，指定一个<c>PackedScene</c>作为用于呈现画面和处理玩家输入的场景，该场景需要自行访问核心来获取数据。
	/// 若不知道如何实现一个功能完备的自定义UI场景，建议使用原厂UI场景
	/// </summary>
	[Export]
	public PackedScene PackedUIScene { get; set; }

	/// <summary>
	/// 游戏资源，指定一个<c>IdleFramework.GameResource</c>，作为定义游戏内容的数据源
	/// </summary>
	[Export]
	public GameResource GameResource { get; set; }

	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationSceneInstantiated:
				Instance = this;
				break;
		}
	}

	public override void _Ready()
	{
		LoadRuntimeBuiltinTranslations();
		if (!InitCheck(out string alertMessage))
		{
			OS.Alert(Localization.Tr("alert.context.idle_framework_launch_failed") + "\n" + alertMessage, Localization.Tr("alert.title.idle_framework_fatal"));
			GetTree().Quit();
			return;
		}
		if (!PlaceUIScene())
		{
			GetTree().Quit();
			return;
		}
	}
	
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// 加载内置运行时翻译至本地化管理器
	/// </summary>
	public static void LoadRuntimeBuiltinTranslations()
	{
		Array<Translation> translations = [];
		foreach (string path in Pathes.RuntimeBuiltinTranslations.Values)
		{
			translations.Add(GD.Load<Translation>(path));
		}
		Localization.LoadRuntimeTranslations(translations);
	}
	
	/// <summary>
	/// 初始化检测，检查本主节点实例的导出属性是否都已被填写，若有为<c>null</c>的则检测不通过
	/// </summary>
	/// <param name="alertMessage">检测不通过时的警告消息，已翻译</param>
	/// <returns>检测通过与否</returns>
	public bool InitCheck(out string alertMessage)
	{
		bool result = true;
		alertMessage = "";
		if (GameResource == null)
		{
			alertMessage += Localization.Tr("alert.context.game_resource_not_specified");
			result = false;
		}
		if (PackedUIScene == null)
		{
			alertMessage += Localization.Tr("alert.context.packed_ui_scene_not_specified");
			result = false;
		}
		return result;
	}
	
	/// <summary>
	/// 实例化并放置UI场景，新的UI场景实例将被添加为子节点，若存在旧的UI场景将被直接<c>QueueFree()</c>
	/// </summary>
	/// <returns>成功与否</returns>
	/// <remarks>若属性<c>PackedUIScene</c>为<c>null</c>则在执行此方法时会出现空引用异常，若<c>PackedUIScene</c>未能实例化为<c>UIScene</c>场景则会直接调用<c>OS.Alert()</c></remarks>
	public bool PlaceUIScene()
	{
		uiScene?.QueueFree();
		uiScene = PackedUIScene.InstantiateOrNull<UIScene>();
		if (uiScene == null)
		{
			OS.Alert(Localization.Tr("alert.context.idle_framework_launch_failed") + "\n" + Localization.Tr("alert.context.packed_ui_scene_not_specified"), Localization.Tr("alert.title.idle_framework_fatal"));
			return false;
		}
		uiScene.MotherNode = this;
		AddChild(uiScene);
		return true;
	}

	/// <summary>
	/// UI场景的实例
	/// </summary>
	public UIScene uiScene;
}
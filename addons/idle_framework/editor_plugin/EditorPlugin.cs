#if TOOLS
using Godot;

namespace IdleFramework.EditorPlugin;

/// <summary>
/// IdleFramework插件主类
/// </summary>
[Tool]
public partial class EditorPlugin : Godot.EditorPlugin
{
	/// <summary>
	/// 编辑器工作区场景打包
	/// </summary>
	public PackedScene EditorWorkspacePackedScene
	{
		get
		{
			_editorWorkspacePackedScene ??= GD.Load<PackedScene>("res://addons/idle_framework/editor_plugin/workspace/workspace.tscn");
			return _editorWorkspacePackedScene;
		}
		set => _editorWorkspacePackedScene = value;
	}
	
	/// <summary>
	/// 编辑器工作区节点
	/// </summary>
	public Control EditorWorkspaceNode
	{
		get => _editorWorkspaceNode;
		set => _editorWorkspaceNode = value;
	}

	public GameResource GameResourceCurrentEditing { get; private set; }

	public override void _Edit(GodotObject @object)
	{
		if (@object is GameResource gameResource)
		{
			GameResourceCurrentEditing = gameResource;
			return;
		}
		GameResourceCurrentEditing = null;
	}

	public override bool _Handles(GodotObject @object)
	{
		return @object is GameResource;
	}

	public override void _EnterTree()
	{
		Localization.LoadTranslations();
		_editorWorkspaceNode = EditorWorkspacePackedScene.Instantiate<Control>();
		if (_editorWorkspaceNode == null)
		{
			GD.PushError("Failed to instantiate EditorWorkspacePackedScene.");
		}
		else
		{
			_editorWorkspaceNode.Visible = false;
			EditorInterface.Singleton.GetEditorMainScreen().AddChild(_editorWorkspaceNode);
		}
		AddInspectorPlugin(_inspector);
	}
	
	public override void _ExitTree()
	{
		_editorWorkspaceNode?.QueueFree();
		_editorWorkspaceNode = null;
		Localization.UnloadTranslations();
		RemoveInspectorPlugin(_inspector);
	}
	
	private PackedScene _editorWorkspacePackedScene;
	private Control _editorWorkspaceNode;
	private Inspector _inspector = new();
	
	public override void _MakeVisible(bool visible) => _editorWorkspaceNode?.Visible = visible;
	public override string _GetPluginName()
	{
		//本来想本地化成"放置引擎"的，结果说是主屏幕插件要开关主屏幕的话必须使用严格准确的Tab名称，而这个虚方法给出的名称会应用于Tab名称，所以不方便本地化
		return "IdleFramework";
	}

	public override Texture2D _GetPluginIcon() => EditorInterface.Singleton.GetEditorTheme().GetIcon("PluginScript", "EditorIcons");
	public override bool _HasMainScreen() => true;
}

#endif

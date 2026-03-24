#if TOOLS
using Godot;

namespace IdleFramework;

/// <summary>
/// IdleFramework插件主类
/// </summary>
[Tool]
public partial class IdleFramework : EditorPlugin
{
	/// <summary>
	/// 编辑器工作区场景打包
	/// </summary>
	public PackedScene EditorWorkspacePackedScene
	{
		get
		{
			_editorWorkspacePackedScene ??= GD.Load<PackedScene>("res://addons/IdleFramework/editor_plugin/workspace/workspace.tscn");
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
	public override void _EnterTree()
	{
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
	}
	public override void _ExitTree()
	{
		_editorWorkspaceNode?.QueueFree();
		_editorWorkspaceNode = null;
	}
	private PackedScene _editorWorkspacePackedScene;
	private Control _editorWorkspaceNode;
	public override void _MakeVisible(bool visible) => _editorWorkspaceNode?.Visible = visible;
	public override string _GetPluginName() => "IdleFramework";
	public override Texture2D _GetPluginIcon() => EditorInterface.Singleton.GetEditorTheme().GetIcon("PluginScript", "EditorIcons");
	public override bool _HasMainScreen() => true;
}

#endif

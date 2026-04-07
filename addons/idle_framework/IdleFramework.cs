#if TOOLS
using Godot;

namespace IdleFramework.EditorPlugin;

/// <summary>
/// IdleFramework插件主类
/// </summary>
[Tool]
public partial class IdleFramework : Godot.EditorPlugin
{
	public override void _EnablePlugin()
	{
		EditorInterface.Singleton.SetPluginEnabled("idle_framework/editor_plugin", true);
	}
	
	public override void _DisablePlugin()
	{
		EditorInterface.Singleton.SetPluginEnabled("idle_framework/editor_plugin", false);
	}
}

#endif

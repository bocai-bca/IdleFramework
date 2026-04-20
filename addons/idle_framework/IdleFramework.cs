#if TOOLS
using Godot;

namespace IdleFramework.EditorPlugin;

/// <summary>
/// IdleFramework插件根类，负责与引擎对接承担插件开关任务
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

using Godot;
using Godot.Collections;

namespace IdleFramework;

/// <summary>
/// 
/// </summary>
[GlobalClass]
public partial class Pathes : RefCounted
{
	public static readonly Dictionary<string, string> RuntimeBuiltinTranslations = new()
	{
		{"en", "res://addons/idle_framework/lang/runtime_builtin.en.translation"},
		{"zh", "res://addons/idle_framework/lang/runtime_builtin.zh.translation"},
	};
}
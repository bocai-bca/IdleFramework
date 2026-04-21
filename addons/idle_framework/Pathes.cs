using System.Collections.Generic;

namespace IdleFramework;

/// <summary>
/// 路径集，容纳了各种路径以供访问，一般情况下请避免在运行时修改
/// </summary>
public static class Pathes
{
	/// <summary>
	/// 内置运行时翻译路径，键为语言标识符，值为对应翻译的Godot项目目录相对路径
	/// </summary>
	public static readonly Dictionary<string, string> RuntimeBuiltinTranslations = new()
	{
		{"en", "res://addons/idle_framework/lang/runtime_builtin.en.translation"},
		{"zh", "res://addons/idle_framework/lang/runtime_builtin.zh.translation"},
	};
}
#if TOOLS
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace IdleFramework.EditorPlugin;

/// <summary>
/// 编辑器插件的本地化管理器
/// </summary>
[Tool]
public class Localization
{
	/// <summary>
	/// 编辑器插件翻译域，通常可以通过访问它的IsEnabled()来获取当前是否已加载翻译
	/// </summary>
	public static TranslationDomain Domain = new();
	
	/// <summary>
	/// 加载翻译至翻译域
	/// </summary>
	public static void LoadTranslations()
	{
		Domain.Enabled = true;
		Domain.AddTranslation(GD.Load<Translation>("res://addons/idle_framework/lang/editor_plugin.en.translation"));
		Domain.AddTranslation(GD.Load<Translation>("res://addons/idle_framework/lang/editor_plugin.zh.translation"));
	}

	/// <summary>
	/// 从翻译域卸载翻译
	/// </summary>
	public static void UnloadTranslations()
	{
		Domain.Clear();
		Domain.Enabled = false;
	}

	/// <summary>
	/// 获取翻译，如果翻译域不可用则返回null、不存在对应键则返回键
	/// </summary>
	/// <param name="key">翻译键</param>
	/// <param name="context">翻译上下文</param>
	/// <returns>已翻译的文本，或者为null或给定键</returns>
	public static string Tr(string key, string context = null)
	{
		if (Domain == null || !Domain.Enabled) return null;
		return Domain.Translate(key, context);
	}
}

#endif
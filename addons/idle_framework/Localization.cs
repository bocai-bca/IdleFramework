using Godot;
using Godot.Collections;

namespace IdleFramework;

/// <summary>
/// 本地化管理器，负责编辑器和运行时的本地化服务。在使用编辑器运行项目时，编辑器和项目实例会分别实例化各自的Localization实例，因此不会冲突或浪费内存
/// </summary>
[Tool]
[GlobalClass]
public partial class Localization : RefCounted
{
	/// <summary>
	/// 编辑器插件翻译域，通常可以通过访问它的IsEnabled()来获取当前是否已加载翻译
	/// </summary>
	public static TranslationDomain Domain = new();
	
	/// <summary>
	/// 加载编辑器翻译至翻译域
	/// </summary>
	public static void LoadEditorTranslations()
	{
		Domain.Enabled = true;
		Domain.AddTranslation(GD.Load<Translation>("res://addons/idle_framework/lang/editor_plugin.en.translation"));
		Domain.AddTranslation(GD.Load<Translation>("res://addons/idle_framework/lang/editor_plugin.zh.translation"));
	}

	/// <summary>
	/// 加载运行时翻译至翻译域
	/// 本方法不仅仅用于加载运行时翻译，它可以加载任何通过手动指定翻译文件的方式提供的翻译
	/// </summary>
	/// <param name="translations">需要加载进翻译域的翻译</param>
	public static void LoadRuntimeTranslations(Array<Translation> translations)
	{
		if (translations is not { Count: > 0 }) return;
		Domain.Enabled = true;
		foreach (Translation translation in translations)
		{
			Domain.AddTranslation(translation);
		}
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
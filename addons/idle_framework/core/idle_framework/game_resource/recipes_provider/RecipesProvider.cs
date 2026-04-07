using System;
using Godot;
using Godot.Collections;

namespace IdleFramework;

/// <summary>
/// 配方提供器，向工厂提供一系列配方
/// </summary>
[GlobalClass]
public partial class RecipesProvider : Resource
{
	/// <summary>
	/// 提供器模式枚举
	/// </summary>
	public enum ProviderMode
	{
		/// <summary>
		/// 黑名单模式，禁用列表中含有的配方
		/// </summary>
		BlackList = 0,
		/// <summary>
		/// 白名单模式，只提供列表中含有的配方
		/// </summary>
		WhiteList = 1,
	}
	
	/// <summary>
	/// 提供器模式
	/// </summary>
	[Export] public ProviderMode Mode { get; set; } = ProviderMode.WhiteList;

	/// <summary>
	/// 提供器的元素数组，可在此填写该填充器的数据表，数据表的行为由提供器模式决定
	/// </summary>
	[Export] public Array<StringName> List { get; set; } = [];

	/// <summary>
	/// 从本提供器中获取配方，需要传入一个配方注册表
	/// </summary>
	/// <param name="recipeRegistry">配方注册表</param>
	/// <exception cref="ArgumentOutOfRangeException">本提供器的Mode是无效值时抛出</exception>
	/// <returns>容纳配方ID的数组</returns>
	public Array<StringName> GetRecipes(Dictionary<StringName, RecipeRegistryObject> recipeRegistry)
	{
		switch (Mode)
		{
			case ProviderMode.BlackList:
				Array<StringName> result = [];
				foreach (StringName recipe in recipeRegistry.Keys)
				{
					if (List.Contains(recipe)) continue;
					result.Add(recipe);
				}
				return result;
			case ProviderMode.WhiteList:
				return List.Duplicate();
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
	
	/// <summary>
	/// 从本提供器中获取配方。将自动从主节点获取游戏资源，如果过程中失败将返回空数组
	/// 性能提示：
	///		如果不嫌麻烦建议主动管理配方注册表的引用然后使用含参数重载，使用本重载可能会比含参数重载多耗费一点性能。本重载是设计给使用者编写脚本更简便用的。
	/// </summary>
	/// <returns>容纳配方ID的数组</returns>
	public Array<StringName> GetRecipes()
	{
		Dictionary<StringName, RecipeRegistryObject> recipeRegistry = MotherNode.Instance?.GameResource?.RecipeRegistry;
		return recipeRegistry == null ? [] : GetRecipes(recipeRegistry);
	}
}
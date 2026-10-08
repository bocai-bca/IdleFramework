using Godot;

namespace IdleFramework;

/// <summary>
/// 配方下单器-可存储型基类。<br/>
/// 可存储型下单器是一类下单器抽象基类，在原始下单器的基础上带有存储一定数量的配方的功能，想要实现相关下单器则需要继承本类。
/// </summary>
[GlobalClass]
public abstract partial class RecipeOrderStorable: RecipeOrder
{
	/// <summary>
	/// 本下单器可以暂存的配方容量，单位为配方个数，如果给定数字小于1则无法添加配方(此部分逻辑由存档数据辅助器实现，这里只是提供个数据壳子)。<br/>
	/// 如果未给定，则通常默认数值为0。<br/>
	/// 本下单器可能在很多时候从本数值提供器获取值，因此建议不要使用太复杂的数值提供器，也尤其避免制作可能造成无限递归的数值提供器链路。<br/>
	/// 本数值提供器包括但不限于在以下时机被调用：<br/>
	///		SaveDataHelper.TryEnqueueRecipeForManualFactory()<br/>
	/// </summary>
	[Export]
	public NumberProvider StoreSize;
}
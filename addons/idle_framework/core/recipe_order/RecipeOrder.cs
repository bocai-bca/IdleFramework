using Godot;

namespace IdleFramework;

/// <summary>
/// 配方下单器的抽象基类
/// </summary>
[GlobalClass]
public abstract partial class RecipeOrder : Resource
{
	/// <summary>
	/// 该配方下单器是否是可手动的。
	/// </summary>
	public abstract bool IsManuallable { get; }
	
	/// <summary>
	/// 拉取配方订单的抽象方法，需要在子类中实现。
	/// </summary>
	/// <returns>该配方下单器本次获取提供的配方ID</returns>
	public abstract string PullRecipe();
}
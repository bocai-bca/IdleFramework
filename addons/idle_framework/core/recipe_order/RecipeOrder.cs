using System.Collections.Generic;
using Godot;

namespace IdleFramework;

/// <summary>
/// 配方下单器的抽象基类
/// </summary>
[GlobalClass]
public abstract partial class RecipeOrder : Resource
{
	/// <summary>
	/// 该配方下单器是否是可手动的，需要由配方下单器子类实现该属性的取值器，请确保其返回结果不可变。
	/// </summary>
	public abstract bool IsManuallable { get; }

	/// <summary>
	/// 拉取配方订单的抽象方法，需要在子类中实现。
	/// </summary>
	/// <param name="argumentsLong">长整型类型的参数表。</param>
	/// <param name="queueString">字符串类型的队列表。</param>
	/// <returns>该配方下单器本次获取提供的配方ID</returns>
	public abstract string PullRecipe(List<long> argumentsLong, Queue<string> queueString);
}
using Godot;

namespace IdleFramework;

/// <summary>
/// 数值提供器的抽象基类
/// </summary>
[GlobalClass]
public abstract partial class NumberProvider : Resource
{
	/// <summary>
	/// 获取值的抽象方法，需要在子类中实现
	/// </summary>
	/// <returns>该数值提供器本次获取提供的值</returns>
	public abstract int GetNumber();
}
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
	public abstract long GetNumber();

	/// <summary>
	/// 该数值提供器的返回值的预览文本，需要保证可在任意线程调用，返回值不会变化、文本简短、不依赖环境的地区语言。
	/// 数值提供器之间可能存在嵌套，此文本也有嵌套获取的可能。
	/// </summary>
	public abstract string NumberPreviewText { get; }
}
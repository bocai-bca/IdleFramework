namespace IdleFramework.Global;

/// <summary>
/// 无限标记值类型，可以用于创建泛型实例来表达数值为无限的数。
/// </summary>
/// <typeparam name="TValue">泛型类型参数。</typeparam>
public struct InfiniteTaggedValue<TValue>(TValue value, bool isInfinite) where TValue : struct
{
	/// <summary>
	/// 存储的原始泛型类型的值。
	/// </summary>
	public TValue Value = value;

	/// <summary>
	/// 是否无限
	/// </summary>
	public bool IsInfinite = isInfinite;

	public static implicit operator InfiniteTaggedValue<TValue>(TValue value) => new(value, false);
}

public static class InfiniteTaggedValueExtension
{
	/// <summary>
	/// 对于为0的无限long，其被认定为0
	/// </summary>
	extension(InfiniteTaggedValue<long>)
	{
		/// <summary>
		/// 求两个数相加，如果两者都不是无限数值，则相当于获得<c>Value</c>为<c>a.Value + b.Value</c>的新实例。
		/// 当两个数皆为无限值时，同符号可得同符号无限值，符号相异得到0。涉及无限值的计算时，<c>Value</c>字段通常会被输出为<c>long.MaxValue</c>或<c>long.MinValue</c>。
		/// </summary>
		/// <param name="a">加数A。</param>
		/// <param name="b">加数B。</param>
		/// <returns>求和后的无限标记long。</returns>
		public static InfiniteTaggedValue<long> operator +(InfiniteTaggedValue<long> a, InfiniteTaggedValue<long> b)
		{
			if (!a.IsInfinite) return b.IsInfinite ? new InfiniteTaggedValue<long>(b.Value >= 0L ? long.MaxValue : long.MinValue, true) : new InfiniteTaggedValue<long>(a.Value + b.Value, false);
			if (!b.IsInfinite) return new InfiniteTaggedValue<long>(a.Value >= 0L ? long.MaxValue : long.MinValue, true);
			if (a.Value >= 0L && b.Value >= 0L) return new InfiniteTaggedValue<long>(long.MaxValue, true);
			if (a.Value < 0L && b.Value < 0L) return new InfiniteTaggedValue<long>(long.MinValue, true);
			return new InfiniteTaggedValue<long>(0L, false);
		}
	}
}
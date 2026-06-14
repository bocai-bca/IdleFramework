using System;

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

		/// <summary>
		/// 求两个数相减，如果两者都不是无限数值，则相当于获得<c>Value</c>为<c>a.Value - b.Value</c>的新实例。
		/// 当两个数皆为无限值时，同符号相减得到0，符号相异得到与被减数同符号的无限值。涉及无限值的计算时，<c>Value</c>字段通常会被输出为<c>long.MaxValue</c>或<c>long.MinValue</c>。
		/// </summary>
		/// <param name="a">被减数A。</param>
		/// <param name="b">减数B。</param>
		/// <returns>相减后的无限标记long。</returns>
		public static InfiniteTaggedValue<long> operator -(InfiniteTaggedValue<long> a, InfiniteTaggedValue<long> b)
		{
			if (!a.IsInfinite) return b.IsInfinite ? new InfiniteTaggedValue<long>(b.Value >= 0L ? long.MinValue : long.MaxValue, true) : new InfiniteTaggedValue<long>(a.Value - b.Value, false);
			if (!b.IsInfinite) return new InfiniteTaggedValue<long>(a.Value >= 0L ? long.MaxValue : long.MinValue, true);
			if (a.Value >= 0L && b.Value >= 0L || a.Value < 0L && b.Value < 0L) return new InfiniteTaggedValue<long>(0L, false);
			if (a.Value >= 0L && b.Value < 0L) return new InfiniteTaggedValue<long>(long.MaxValue, true);
			return new InfiniteTaggedValue<long>(long.MinValue, true);
		}

		/// <summary>
		/// 对两个操作数做大于号判断。
		/// </summary>
		/// <param name="a">操作数A。</param>
		/// <param name="b">操作数B。</param>
		/// <returns>大于号判断结果。</returns>
		public static bool operator >(InfiniteTaggedValue<long> a, InfiniteTaggedValue<long> b)
		{
			if (a.IsInfinite) // A无限
			{
				if (!b.IsInfinite) // A无限，B有限
					return a.Value >= 0L;
				// AB无限
				if (a.Value >= 0L) // A正无限，B无限
					return b.Value < 0L;
				// A负无限，B无限
				return false;
			}
			// A有限
			if (b.IsInfinite) // A有限，B无限
				return b.Value < 0L;
			// AB有限
			return a.Value > b.Value;
		}

		/// <summary>
		/// 对两个操作数做小于号判断。
		/// </summary>
		/// <param name="a">操作数A。</param>
		/// <param name="b">操作数B。</param>
		/// <returns>小于号判断结果。</returns>
		public static bool operator <(InfiniteTaggedValue<long> a, InfiniteTaggedValue<long> b)
		{
			// 就是把a和b对调的大于判断
			if (b.IsInfinite)
			{
				if (!a.IsInfinite) return b.Value >= 0L;
				if (b.Value >= 0L) return a.Value < 0L;
				return false;
			}
			if (a.IsInfinite) return a.Value < 0L;
			return b.Value > a.Value;
		}

		/// <summary>
		/// 对两个操作数作双等于判断。
		/// 如果两者皆为无限数，则<c>Value</c>部分符号相同时才判定为等于，<c>0</c>被认为是正数。
		/// </summary>
		/// <param name="a">操作数A。</param>
		/// <param name="b">操作数B。</param>
		/// <returns>给定的两个操作数在逻辑上是否相等。</returns>
		public static bool operator ==(InfiniteTaggedValue<long> a, InfiniteTaggedValue<long> b)
		{
			if (a.IsInfinite && b.IsInfinite) return (a.Value >= 0L && b.Value >= 0L) || (a.Value < 0L && b.Value < 0L);
			return a.Value == b.Value;
		}

		/// <summary>
		/// 对两个操作数作不等于判断。
		/// 如果两者皆为无限数，则<c>Value</c>部分符号相同时才判定为等于，<c>0</c>被认为是正数。
		/// </summary>
		/// <param name="a">操作数A。</param>
		/// <param name="b">操作数B。</param>
		/// <returns>给定的两个操作数在逻辑上是否不相等。</returns>
		public static bool operator !=(InfiniteTaggedValue<long> a, InfiniteTaggedValue<long> b)
		{
			if (a.IsInfinite && b.IsInfinite) return (a.Value >= 0L && b.Value < 0L) || (a.Value < 0L && b.Value >= 0L);
			return a.Value != b.Value;
		}

		/// <summary>
		/// 对两个操作数作大于等于判断。其实际逻辑为<c>a == b || a &gt; b</c>。
		/// </summary>
		/// <param name="a">操作数A。</param>
		/// <param name="b">操作数B。</param>
		/// <returns>给定的两个操作数在逻辑上是否满足A大于等于B。</returns>
		public static bool operator >=(InfiniteTaggedValue<long> a, InfiniteTaggedValue<long> b) => a == b || a > b;

		/// <summary>
		/// 对两个操作数作小于等于判断。其实际逻辑为<c>a == b || a &lt; b</c>。
		/// </summary>
		/// <param name="a">操作数A。</param>
		/// <param name="b">操作数B。</param>
		/// <returns>给定的两个操作数在逻辑上是否满足A小于等于B。</returns>
		public static bool operator <=(InfiniteTaggedValue<long> a, InfiniteTaggedValue<long> b) => a == b || a < b;

		/// <summary>
		/// 求最小值。
		/// 涉及无限值的计算时，<c>Value</c>字段通常会被输出为<c>long.MaxValue</c>或<c>long.MinValue</c>。
		/// </summary>
		/// <param name="a">操作数A。</param>
		/// <param name="b">操作数B。</param>
		/// <returns>A与B中较小的值。</returns>
		public static InfiniteTaggedValue<long> Min(InfiniteTaggedValue<long> a, InfiniteTaggedValue<long> b)
		{
			if (a.IsInfinite)
			{
				// A无限
				if (!b.IsInfinite)
					// A无限，B有限
					return a.Value >= 0L ?
						// A正无限，B有限
						new InfiniteTaggedValue<long>(b.Value, false) :
						// A负无限，B有限
						new InfiniteTaggedValue<long>(long.MinValue, true);
				// A无限，B无限
				if (a.Value < 0 || b.Value < 0)
					// AB皆无限，且至少有一个为负无限
					return new InfiniteTaggedValue<long>(long.MinValue, true);
				// AB皆无限，且没有一个为负无限，即皆为正无限
				return new InfiniteTaggedValue<long>(long.MaxValue, true);
			}
			// A有限
			if (b.IsInfinite)
			{
				// A有限，B无限
				return b.Value < 0L ?
					// A有限，B负无限
					new InfiniteTaggedValue<long>(long.MinValue, true) :
					// A有限，B正无限
					new InfiniteTaggedValue<long>(a.Value, false);
			}
			// A有限，B有限
			return new InfiniteTaggedValue<long>(Math.Min(a.Value, b.Value), false);
		}

		public static InfiniteTaggedValue<long> Max(InfiniteTaggedValue<long> a, InfiniteTaggedValue<long> b)
		{
			if (a.IsInfinite)
			{
				// A无限
				if (!b.IsInfinite)
					// A无限，B有限
					return a.Value >= 0L ?
						// A正无限，B有限
						new InfiniteTaggedValue<long>(long.MaxValue, true) :
						// A负无限，B有限
						new InfiniteTaggedValue<long>(b.Value, false);
				// A无限，B无限
				if (a.Value >= 0 || b.Value >= 0)
					// AB皆无限，且至少有一个为正无限
					return new InfiniteTaggedValue<long>(long.MaxValue, true);
				// AB皆无限，且没有一个为正无限，即皆为负无限
				return new InfiniteTaggedValue<long>(long.MinValue, true);
			}
			// A有限
			if (b.IsInfinite)
			{
				// A有限，B无限
				return b.Value < 0L ?
					// A有限，B负无限
					new InfiniteTaggedValue<long>(a.Value, false) :
					// A有限，B正无限
					new InfiniteTaggedValue<long>(long.MaxValue, true);
			}
			// A有限，B有限
			return new InfiniteTaggedValue<long>(Math.Max(a.Value, b.Value), false);
		}
		
		// Provided by Deepseek LLM.
		/// <summary>
		/// 将值 <paramref name="from"/> 向目标 <paramref name="to"/> 移动不超过 <paramref name="delta"/> 的距离。
		/// 涉及无限值时，有限步长只能到达边界值（<c>long.MaxValue</c>/<c>long.MinValue</c>），无限步长可到达对方无限值。
		/// 返回的无限值实例总是将 <c>Value</c> 规范化为边界值。
		/// </summary>
		/// <param name="from">当前值。</param>
		/// <param name="to">目标值。</param>
		/// <param name="delta">最大移动步长（可为负）。</param>
		/// <returns>移动后的新实例。</returns>
		/// <remarks>This method was provided by Deepseek LLM.</remarks>
		public static InfiniteTaggedValue<long> MoveToward(InfiniteTaggedValue<long> from, InfiniteTaggedValue<long> to, InfiniteTaggedValue<long> delta)
		{
		    if (!from.IsInfinite && !to.IsInfinite)
		    {
		        long diff = to.Value - from.Value, step = delta.IsInfinite ? (delta.Value >= 0 ? long.MaxValue : long.MinValue) : delta.Value;
		        if (step >= 0) return new InfiniteTaggedValue<long>(from.Value + (diff >= 0 ? Math.Min(diff, step) : -Math.Min(-diff, step)), false);
		        long absStep = -step;
		        return new InfiniteTaggedValue<long>(from.Value + (diff <= 0 ? Math.Min(-diff, absStep) : -Math.Min(diff, absStep)), false);
		    }
		    bool toPos = to.Value >= 0;
		    if (!from.IsInfinite)
		    {
		        if (delta.IsInfinite && ((delta.Value >= 0) == toPos)) return new InfiniteTaggedValue<long>(toPos ? long.MaxValue : long.MinValue, true);
		        if (delta.IsInfinite) return new InfiniteTaggedValue<long>(from.Value, false);
		        long step = delta.Value, newVal = from.Value + (toPos ? step : -step);
		        if (newVal >= long.MaxValue) return new InfiniteTaggedValue<long>(long.MaxValue, true);
		        return newVal <= long.MinValue ? new InfiniteTaggedValue<long>(long.MinValue, true) : new InfiniteTaggedValue<long>(newVal, false);
		    }
		    if (!to.IsInfinite) return delta.IsInfinite ? new InfiniteTaggedValue<long>(to.Value, false) : new InfiniteTaggedValue<long>(from.Value >= 0 ? long.MaxValue : long.MinValue, true);
		    bool fromPos = from.Value >= 0;
		    if (fromPos == toPos) return new InfiniteTaggedValue<long>(fromPos ? long.MaxValue : long.MinValue, true);
		    return delta.IsInfinite ? new InfiniteTaggedValue<long>(toPos ? long.MaxValue : long.MinValue, true) : new InfiniteTaggedValue<long>(fromPos ? long.MaxValue : long.MinValue, true);
		}
	}
}
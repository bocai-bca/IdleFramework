using System;
using System.Linq;

namespace IdleFramework.Global;

/// <summary>
/// 随机器工具
/// </summary>
public static class RandomTool
{
	/// <summary>
	/// 权重选择，从数个对象中选择其一。
	/// </summary>
	/// <param name="weights">权重数组。如果元素为负数，会导致意料之外的情况。如果元素总额超过<c>long.MaxValue</c>，会被钳制在<c>long.MaxValue</c>。</param>
	/// <param name="randomInstance">使用的<c>Random</c>实例。</param>
	/// <returns>随机结果，为权重数组的一个索引。</returns>
	public static int RandomWeighted(this long[] weights, Random randomInstance)
	{
		long i = weights.Aggregate(0L, (current, weight) => current + Math.Clamp(weight, 0L, long.MaxValue - current - 1L));
		long random = randomInstance.NextInt64(0L, i); // random = 0..(max-1)
		i = 0L;
		for (int index = 0; index < weights.Length; index++)
		{
			i += weights[index];
			if (random < i) return index;
		}
		return 0;
	}
}
using System;
using System.Linq;
using Godot;
using Godot.Collections;

namespace IdleFramework;

/// <summary>
/// 数值提供器-求和。
/// 提供一个列表，返回这个列表中所有数值提供器的结果之和。
/// 使用时请注意避免自引用造成无限递归。
/// </summary>
[GlobalClass]
public partial class NumberProviderSum : NumberProvider
{
	/// <summary>
	/// 待求和的数值提供器列表
	/// </summary>
	[Export]
	[ExportGroup("Data")]
	public Array<NumberProvider> SumList { get; set; }
	
	public override int GetNumber()
	{
		if (recursionLock)
		{
			Logger.LogError("log.error.number_provider_sum.unexcepted_call_blocked_by_recursion_lock");
			return 0;
		}
		recursionLock = true;
		int result;
		try
		{
			result = SumList.Sum(provider => provider.GetNumber());
		}
		catch (Exception e)
		{
			Logger.LogError("log.error.number_provider_sum.catched_exception_on_calling_number_providers_in_sum_list" + e);
			recursionLock = false;
			throw;
		}
		recursionLock = false;
		return result;
	}

	private bool recursionLock;
}
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
	
	public override long GetNumber()
	{
		if (recursionLock)
		{
			Logger.LogError(Localization.Tr("log.error.number_provider_sum.unexcepted_call_blocked_by_recursion_lock"));
			return 0L;
		}
		recursionLock = true;
		long result;
		try
		{
			result = SumList.Sum(provider => provider.GetNumber());
		}
		catch (OverflowException)
		{
			Logger.LogWarning(Localization.Tr("log.error.number_provider_sum.overflow"));
			recursionLock = false;
			return long.MaxValue;
		}
		catch (Exception e)
		{
			Logger.LogError(string.Format(Localization.Tr("log.error.number_provider_sum.catched_exception_on_calling_number_providers_in_sum_list"), e));
			recursionLock = false;
			return 0L;
		}
		recursionLock = false;
		return result;
	}

	public override long NumberPreviewAvg => SumList.Count == 0 ? 0L : SumList.Sum(t => t.NumberPreviewAvg);

	public override string NumberPreviewText
	{
		get
		{
			if (SumList.Count == 0) return "0";
			if (recursionLock)
			{
				Logger.LogError(Localization.Tr("log.error.number_provider_sum.unexcepted_call_blocked_by_recursion_lock"));
				return "0";
			}
			recursionLock = true;
			string result = "";
			try
			{
				for (int i = 0; i < SumList.Count; i++)
				{
					string subText = SumList[i].NumberPreviewText;
					if (subText.Contains('~') || subText.Contains('+')) subText = "(" + subText + ")";
					result += subText;
					if (i != SumList.Count - 1) result += "+";
				}
			}
			catch (Exception e)
			{
				Logger.LogError(Localization.Tr("log.error.number_provider_sum.catched_exception_on_calling_number_providers_in_sum_list" + e));
				recursionLock = false;
				return "0";
			}
			recursionLock = false;
			return result;
		}
	}

	private bool recursionLock;
}
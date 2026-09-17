using System;

namespace IdleFramework.Global;

/// <summary>
/// 数字格式化工具
/// </summary>
public static class NumberFormator
{
	/// <summary>
	/// 将数字转换到短文本。
	/// </summary>
	/// <param name="number">要转换的数字。</param>
	/// <param name="digits">要保留的小数位数。</param>
	/// <param name="suffixes">尾缀表，如果传入<c>null</c>则使用默认尾缀表。必须至少有一个元素。</param>
	/// <returns>转换的结果</returns>
	public static string NumberToShortText(this long number, uint digits = 1, string[] suffixes = null)
	{
		string result = string.Empty;
		if (number < 0) result += "-";
		number = Math.Abs(number);
		suffixes ??= ["", "k", "M", "B", "T", "P", "E"];
		if (number < 1_000L)
		{
			return result + number + suffixes[0];
		}
		int suffixIndex = 0;
		double scaled = number;
		while (scaled >= 1_000D && suffixIndex < suffixes.Length - 1)
		{
			scaled /= 1_000D;
			suffixIndex++;
		}
		return result + scaled.ToString("F" + digits) + suffixes[suffixIndex];
	}

	/// <summary>
	/// 将代表秒数的数字转换到短文本。
	/// </summary>
	/// <param name="seconds">要转换的秒数。</param>
	/// <param name="suffixes">尾缀表，如果传入<c>null</c>则使用默认尾缀表。必须拥有四个元素。</param>
	/// <returns></returns>
	public static string TimeToShortText(this long seconds, string[] suffixes = null)
	{
		string result = seconds < 0L ? "- " : string.Empty;
		seconds = Math.Abs(seconds);
		suffixes ??= ["s", "m", "h", "d"];
		TimeSpan timeSpan = new(seconds * TimeSpan.TicksPerSecond);
		if (timeSpan.Days >= 1)
		{
			result += timeSpan.Days + suffixes[3];
			if (timeSpan.Hours != 0)
			{
				result += " " + timeSpan.Hours + suffixes[2];
			}
			return result;
		}
		if (timeSpan.Hours >= 1)
		{
			result += timeSpan.Hours + suffixes[2];
			if (timeSpan.Minutes != 0)
			{
				result += " " + timeSpan.Minutes + suffixes[1];
			}
			return result;
		}
		if (timeSpan.Minutes >= 1)
		{
			result += timeSpan.Minutes + suffixes[1];
			if (timeSpan.Seconds != 0)
			{
				result += " " + timeSpan.Seconds + suffixes[0];
			}
			return result;
		}
		return timeSpan.Seconds + suffixes[0];
	}
}
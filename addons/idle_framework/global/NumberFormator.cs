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
}
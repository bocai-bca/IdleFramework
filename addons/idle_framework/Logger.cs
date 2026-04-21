using System;
using Godot;

namespace IdleFramework;

/// <summary>
/// IdleFramework的日志打印器，负责输出优美有型的文本
/// </summary>
public static class Logger
{
	/// <summary>
	/// 打印标准文本，适用于具有重要性的低频普通文本，会一并打印当前系统时间和引擎帧数
	/// </summary>
	/// <param name="text">要打印到输出的文本</param>
	public static void LogInfo(string text)
	{
		GD.Print(GetTimeFramesString() + "INFO> " + text);
	}

	/// <summary>
	/// 打印快速文本，适用于不重要的高频普通文本，不会打印系统时间和引擎帧数，性能相比<c>LogInfo()</c>更好
	/// </summary>
	/// <param name="text">要打印到输出的文本</param>
	public static void LogFaster(string text)
	{
		GD.Print("INFO> " + text);
	}
	
	/// <summary>
	/// 打印警告文本，适用于具有重要性的低频警告文本，会一并打印当前系统时间和引擎帧数
	/// </summary>
	/// <param name="text">要打印到输出的文本</param>
	public static void LogWarning(string text)
	{
		GD.PushWarning(GetTimeFramesString() + EngineFramesToBase36(Engine.GetProcessFrames()) + "WARN> " + text);
	}
	
	/// <summary>
	/// 打印错误文本，适用于具有重要性的低频错误文本，会一并打印当前系统时间和引擎帧数
	/// </summary>
	/// <param name="text">要打印到输出的文本</param>
	public static void LogError(string text)
	{
		GD.PushError(GetTimeFramesString() + "ERROR> " + text);
	}

	/// <summary>
	/// <c>Logger</c>自己使用的获取格式化的时间、帧数字符串的方法
	/// </summary>
	/// <returns>含时间和帧数的格式化字符串</returns>
	public static string GetTimeFramesString()
	{
		DateTime dateTime = DateTime.Now;
		return "[" + dateTime.Hour + ":" + dateTime.Minute + ":" + dateTime.Second + "|" + EngineFramesToBase36(Engine.GetProcessFrames()) + "]";
	}

	/// <summary>
	/// <c>Logger</c>自己使用的将引擎帧数转换为Base36的方法
	/// </summary>
	/// <param name="frames">引擎帧数，应当通过<c>Engine.GetProcessFrames()</c>获取</param>
	/// <returns>转换好的Base36字符串，使用0-9、A-Z范围的字符表示传入的数字</returns>
	public static string EngineFramesToBase36(ulong frames)
	{
		if (frames == 0) return "0";
		const string CHARS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		Span<char> charSpan = stackalloc char[13];
		byte index = 13;
		while (frames > 0 && index > 0)
		{
			index -= 1;
			charSpan[index] = CHARS[(int)(frames % 36)];
			frames /= 36;
		}
		return charSpan[index..].ToString();
	}
}
namespace IdleFramework;

/// <summary>
/// 全名为LoggerTranslation，封装先翻译再日志的日志辅助类，旨在缩短每次日志时的代码字符量，提升易读性。
/// </summary>
public static class LogTr
{
	public static void Info(string trKey) => Logger.LogInfo(Localization.Tr(trKey));

	public static void Info(string trKey, params string[] args) => Logger.LogInfo(string.Format(Localization.Tr(trKey), args));

	public static void Warning(string trKey) => Logger.LogWarning(Localization.Tr(trKey));

	public static void Warning(string trKey, params string[] args) => Logger.LogWarning(string.Format(Localization.Tr(trKey), args));

	public static void Error(string trKey) => Logger.LogError(Localization.Tr(trKey));

	public static void Error(string trKey, params string[] args) => Logger.LogError(string.Format(Localization.Tr(trKey), args));

	public static void Faster(string trKey) => Logger.LogFaster(Localization.Tr(trKey));

	public static void Faster(string trKey, params string[] args) => Logger.LogFaster(string.Format(Localization.Tr(trKey), args));
}
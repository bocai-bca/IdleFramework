using System;
using System.IO;
using System.Threading.Tasks;
using Godot;

namespace IdleFramework.Core;

/// <summary>
/// 存档访问，在运行时供UI场景和运行时类读写存档。提供以存档为单位级别的API，多数操作不需要访问游戏资源。
/// </summary>
public static class SaveAccess
{
	/// <summary>
	/// 任务结果枚举
	/// </summary>
	public enum WorkResult
	{
		/// <summary>
		/// 工作成功
		/// </summary>
		Success,
		/// <summary>
		/// 其他错误
		/// </summary>
		OtherError,
		/// <summary>
		/// 未找到存档
		/// </summary>
		SaveNotFound,
		/// <summary>
		/// 存档解析失败
		/// </summary>
		SaveParsingFailed,
		/// <summary>
		/// 文件读写失败
		/// </summary>
		FileIOFailed,
		/// <summary>
		/// 寻找最后存档失败
		/// </summary>
		FindLatestSaveFailed,
	}
	
	/// <summary>
	/// 默认存档目录
	/// </summary>
	public const string DEFAULT_SAVE_DIR = "user://saves";

	/// <summary>
	/// 当前是否正在多线程工作(如读写内存中的存档)
	/// </summary>
	public static bool IsMultiThreadWorking => !(WorkingTask == null || WorkingTask.IsCompleted);

	/// <summary>
	/// 工作线程
	/// </summary>
	public static Task<WorkResult> WorkingTask { get; private set; }

	/// <summary>
	/// 已加载的数据的辅助类。
	/// </summary>
	public static SaveDataHelper LoadedDataHelper { get; set; }

	/// <summary>
	/// 加载给定游戏ID的最后存档，如果成功加载则可以通过<c>LoadedDatas</c>搭配该游戏ID作为键来获取该存档的<c>SaveData</c>实例。
	/// 出于线程安全考虑，建议在所有情况下使用<c>LoadLatestSaveForGameAsync()</c>。本方法主要供<c>SaveAccess</c>工作线程使用。
	/// </summary>
	/// <param name="gameResource">要加载存档的游戏资源。</param>
	/// <param name="saveDir">存档起始目录。</param>
	/// <returns>任务结果。</returns>
	public static WorkResult LoadLatestSaveForGame(GameResource gameResource, string saveDir = DEFAULT_SAVE_DIR) //工作线程方法，不要经过Safety方法而是使用lock直接访问线程保护成员
	{
		string gameID = gameResource.GameID;
		string[] savePathes = GetSavesForGame(gameID, saveDir);
		if (savePathes.IsEmpty()) return WorkResult.SaveNotFound;
		string latestSavePath = "";
		ulong latestSavePathNameNum = 0L;
		foreach (string savePath in savePathes)
		{
			ulong currentSavePathNameNum = Convert.ToUInt64(savePath.GetFile());
			if (currentSavePathNameNum <= latestSavePathNameNum) continue;
			latestSavePathNameNum = currentSavePathNameNum;
			latestSavePath = savePath;
		}
		if (latestSavePath == "")
		{
			Logger.LogError(string.Format(Localization.Tr("log.error.save_access.failed_to_get_latest_save"), gameID));
			return WorkResult.FindLatestSaveFailed;
		}
		string saveContent;
		try
		{
			saveContent = File.ReadAllText(latestSavePath);
		}
		catch (Exception e)
		{
			Logger.LogError(Localization.Tr("log.error.save_access.failed_to_read_save_file") + " " + e.Message);
			return WorkResult.FileIOFailed;
		}
		SaveData saveDataParsed = SaveData.ParseFromJsonText(saveContent);
		if (saveDataParsed == null)
		{
			Logger.LogError(Localization.Tr("log.error.save_access.save_data_parser_returned_null"));
			return WorkResult.SaveParsingFailed;
		}
		LoadedDataHelper = new SaveDataHelper(gameResource, saveDataParsed);
		return WorkResult.Success;
	}

	/// <summary>
	/// 启动多线程加载给定游戏资源的最后存档，作用详见<c>LoadLatestSaveForGame()</c>。
	/// 本类型的工作线程只能同时做一件事，如果<c>SaveAccess.WorkingTask</c>已在工作中，则调用此方法时会阻塞调用方线程直到工作线程完成上一轮工作。
	/// 返回值可以不await直接丢弃，后续可访问<c>SaveAccess.IsMultiThreadWorking</c>属性获悉异步工作是否完成。
	/// </summary>
	/// <param name="gameResource">要加载存档的游戏资源。</param>
	/// <param name="saveDir">存档起始目录</param>
	/// <returns>新启动的<c>Task&lt;WorkResult&gt;</c>实例，亦可以从<c>SaveAccess.WorkingTask</c>属性获得</returns>
	public static async Task<WorkResult> LoadLatestSaveForGameAsync(GameResource gameResource, string saveDir = DEFAULT_SAVE_DIR) //含等待方法，请勿在工作线程中使用它
	{
		if (IsMultiThreadWorking) WorkingTask.Wait();
		WorkingTask = Task.Run(() => LoadLatestSaveForGame(gameResource, saveDir));
		return await WorkingTask;
	}

	/// <summary>
	/// 为给定游戏ID创建存档并载入内存，新建的存档可通过<c>LoadedDatas</c>访问，但此方法不会直接向硬盘写入新存档，关于向硬盘写入存档文件详见<c>StoreSaveForGame()</c>。
	/// 出于线程安全考虑，建议在所有情况下使用<c>CreateSaveForGameAsync()</c>。
	/// </summary>
	/// <param name="gameResource">要创建存档的游戏资源。</param>
	/// <returns>任务结果，只会返回<c>SaveAccess.WorkResult.Success</c>。</returns>
	public static WorkResult CreateSaveForGame(GameResource gameResource) //工作线程方法，不要经过Safety方法而是使用lock直接访问线程保护成员
	{
		Logger.LogInfo(string.Format(Localization.Tr("log.info.save_access.creating_new_save_for_game"), gameResource.GameID));
		LoadedDataHelper = new SaveDataHelper(gameResource, new SaveData());
		LoadedDataHelper.InitWholeSave();
		return WorkResult.Success;
	}

	/// <summary>
	/// 启动多线程为给定游戏ID创建存档并载入内存，新建的存档可通过<c>LoadedDatas</c>访问，但此方法不会直接向硬盘写入新存档，关于向硬盘写入存档文件详见<c>StoreSaveForGame()</c>。
	/// 本类型的工作线程只能同时做一件事，如果<c>SaveAccess.WorkingTask</c>已在工作中，则调用此方法时会阻塞调用方线程直到工作线程完成上一轮工作。
	/// 返回值可以不await直接丢弃，后续可访问<c>SaveAccess.IsMultiThreadWorking</c>属性获悉异步工作是否完成。
	/// </summary>
	/// <param name="gameResource">要创建存档的游戏资源。</param>
	/// <returns>新启动的<c>Task&lt;WorkResult&gt;</c>实例，亦可以从<c>SaveAccess.WorkingTask</c>属性获得</returns>
	public static async Task<WorkResult> CreateSaveForGameAsync(GameResource gameResource) //含等待方法，请勿在工作线程中使用它
	{
		if (IsMultiThreadWorking) WorkingTask.Wait();
		WorkingTask = Task.Run(() => CreateSaveForGame(gameResource));
		return await WorkingTask;
	}

	/// <summary>
	/// 获取指定游戏ID的所有存档
	/// </summary>
	/// <param name="gameID">要获取存档的游戏ID</param>
	/// <param name="saveDir">存档起始目录</param>
	/// <returns>该游戏ID的所有存档文件的路径，如果没有取得任何结果则返回空数组</returns>
	public static string[] GetSavesForGame(string gameID, string saveDir = DEFAULT_SAVE_DIR)
	{
		string saveDirCombined = saveDir.PathJoin(gameID);
		return !Directory.Exists(saveDirCombined) ? [] : Directory.GetFiles(saveDirCombined, "*.ifs", SearchOption.TopDirectoryOnly);
	}
}
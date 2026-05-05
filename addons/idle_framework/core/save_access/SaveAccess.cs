using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Godot;

namespace IdleFramework.Core;

/// <summary>
/// 存档访问，在运行时供UI场景和运行时类读写存档。
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
	/// 已加载的数据，键为游戏ID，值为存档数据。请通过<c>GetDataDuplicatedAwait()</c>和<c>SetDataDuplicatedAwait()</c>访问该字典中的数据。
	/// 任何时候直接访问本字典都需要使用lock。
	/// </summary>
	private static Dictionary<string, SaveData> LoadedDatas { get; } = [];

	/// <summary>
	/// 可等待地从已加载存档数据字典中取值并复制独立实例，如果当前本类的工作线程正在执行，则会阻塞调用方线程。
	/// 通过本方法获取的<c>SaveData</c>实例不存在于字典中，对其进行修改不会影响字典内的原实例，适合用于需要临时获取、改动值的场合。
	/// 具有一定性能开销，请考虑降低使用频率。
	/// </summary>
	/// <param name="key">要索引的键。</param>
	/// <param name="saveData">获取到的存档数据的复制品实例。</param>
	/// <param name="duplicate">是否复制实例，如果为<c>false</c>则将返回字典中原始实例的引用。为确保多线程安全，建议在所有情况下都保持<c>true</c>来获取复制品实例。</param>
	/// <returns>成功与否，例如若<c>LoadedDatas</c>中不存在给定的<c>key</c>则返回<c>false</c>test。</returns>
	public static bool GetDataSafety(string key, out SaveData saveData, bool duplicate = true) //含等待方法，请勿在工作线程中使用它
	{
		if (IsMultiThreadWorking) WorkingTask.Wait();
		lock (loadedDatasLock)
		{
			if (!LoadedDatas.TryGetValue(key, out saveData)) return false;
			if (duplicate) saveData = saveData.Duplicate();
		}
		return true;
	}

	/// <summary>
	/// 可等待地向已加载存档数据字典中存储给定<c>SaveData</c>实例的复制品，如果当前本类的工作线程正在执行，则会阻塞调用方线程。
	/// 本方法存入字典的实例是与实参传入的实例相互独立的，后续修改原先传入的实例不会影响已存入字典的实例。
	/// 具有一定性能开销，请考虑降低使用频率。
	/// </summary>
	/// <param name="key">要存储的键。</param>
	/// <param name="saveData">要存储的存档数据实例。</param>
	/// /// <param name="duplicate">是否复制实例，如果为<c>false</c>则将向字典存入原始实例的引用。为确保多线程安全，建议在所有情况下都保持<c>true</c>来存入复制品实例。</param>
	/// <returns>成功与否，总是返回true</returns>
	public static bool SetDataSafety(string key, SaveData saveData, bool duplicate = true) //含等待方法，请勿在工作线程中使用它
	{
		if (IsMultiThreadWorking) WorkingTask.Wait();
		lock (loadedDatasLock)
		{
			LoadedDatas[key] = duplicate ? saveData.Duplicate() : saveData;
		}
		return true;
	}

	/// <summary>
	/// <c>GetDataSafety()</c>的无线程安全设计版本，与其的区别是本方法不会等待工作线程完成、不经过互斥锁、返回原始实例引用。设计于调试时手动调用，<c>SaveAccess</c>自身不会使用它。
	/// 具有潜在的高危险性，除非你真正知道自己在做什么，否则不要使用！
	/// </summary>
	/// <param name="key">要索引的键。</param>
	/// <param name="saveData">获取到的存档数据的复制品实例。</param>
	/// <returns>成功与否，例如若<c>LoadedDatas</c>中不存在给定的<c>key</c>则返回<c>false</c>test。</returns>
	public static bool GetDataUnsafety(string key, out SaveData saveData)
	{
		return LoadedDatas.TryGetValue(key, out saveData);
	}
	
	/// <summary>
	/// 加载给定游戏ID的最后存档，如果成功加载则可以通过<c>LoadedDatas</c>搭配该游戏ID作为键来获取该存档的<c>SaveData</c>实例。
	/// 出于线程安全考虑，建议在所有情况下使用<c>LoadLatestSaveForGameAsync()</c>。本方法主要供<c>SaveAccess</c>工作线程使用。
	/// </summary>
	/// <param name="gameID">要加载存档的游戏ID。</param>
	/// <param name="saveDir">存档起始目录。</param>
	/// <returns>任务结果。</returns>
	public static WorkResult LoadLatestSaveForGame(string gameID, string saveDir = DEFAULT_SAVE_DIR) //工作线程方法，不要经过Safety方法而是使用lock直接访问线程保护成员
	{
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
		lock (loadedDatasLock)
		{
			LoadedDatas[gameID] = saveDataParsed;
		}
		return WorkResult.Success;
	}

	/// <summary>
	/// 启动多线程加载给定游戏ID的最后存档，作用详见<c>LoadLatestSaveForGame()</c>。
	/// 本类型的工作线程只能同时做一件事，如果<c>SaveAccess.WorkingTask</c>已在工作中，则调用此方法时会阻塞调用方线程直到工作线程完成上一轮工作。
	/// 返回值可以不await直接丢弃，后续可访问<c>SaveAccess.IsMultiThreadWorking</c>属性获悉异步工作是否完成。
	/// </summary>
	/// <param name="gameID">要加载存档的游戏ID</param>
	/// <param name="saveDir">存档起始目录</param>
	/// <returns>新启动的<c>Task&lt;WorkResult&gt;</c>实例，亦可以从<c>SaveAccess.WorkingTask</c>属性获得</returns>
	public static async Task<WorkResult> LoadLatestSaveForGameAsync(string gameID, string saveDir = DEFAULT_SAVE_DIR) //含等待方法，请勿在工作线程中使用它
	{
		if (IsMultiThreadWorking) WorkingTask.Wait();
		WorkingTask = Task.Run(() => LoadLatestSaveForGame(gameID, saveDir));
		return await WorkingTask;
	}

	/// <summary>
	/// 为给定游戏ID创建存档并载入内存，新建的存档可通过<c>LoadedDatas</c>访问，但此方法不会直接向硬盘写入新存档，关于向硬盘写入存档文件详见<c>StoreSaveForGame()</c>。
	/// 出于线程安全考虑，建议在所有情况下使用<c>CreateSaveForGameAsync()</c>。
	/// </summary>
	/// <param name="gameResource">要创建存档的游戏资源。</param>
	/// <param name="gameVersion">游戏版本。</param>
	/// <returns>任务结果，只会返回<c>SaveAccess.WorkResult.Success</c>。</returns>
	public static WorkResult CreateSaveForGame(GameResource gameResource, int gameVersion) //工作线程方法，不要经过Safety方法而是使用lock直接访问线程保护成员
	{
		SaveData newSave = new()
		{
			GameID = gameResource.GameID,
			GameVersion = gameVersion,
			LastUpdateUtcTick = TimeHelper.GetUtcNowTick(),
		};
		foreach ((string key, SpaceRegistryObject spaceRegistryObject) in gameResource.SpaceRegistry)
		{
			newSave.InstantiateSpaceRegistryObject(key, gameResource);
		}
		lock (loadedDatasLock)
		{
			LoadedDatas[gameResource.GameID] = newSave;
		}
		return WorkResult.Success;
	}

	/// <summary>
	/// 启动多线程为给定游戏ID创建存档并载入内存，新建的存档可通过<c>LoadedDatas</c>访问，但此方法不会直接向硬盘写入新存档，关于向硬盘写入存档文件详见<c>StoreSaveForGame()</c>。
	/// 本类型的工作线程只能同时做一件事，如果<c>SaveAccess.WorkingTask</c>已在工作中，则调用此方法时会阻塞调用方线程直到工作线程完成上一轮工作。
	/// 返回值可以不await直接丢弃，后续可访问<c>SaveAccess.IsMultiThreadWorking</c>属性获悉异步工作是否完成。
	/// </summary>
	/// <param name="gameResource">要创建存档的游戏资源。</param>
	/// <param name="gameVersion">游戏版本。</param>
	/// <returns>新启动的<c>Task&lt;WorkResult&gt;</c>实例，亦可以从<c>SaveAccess.WorkingTask</c>属性获得</returns>
	public static async Task<WorkResult> CreateSaveForGameAsync(GameResource gameResource, int gameVersion) //含等待方法，请勿在工作线程中使用它
	{
		if (IsMultiThreadWorking) WorkingTask.Wait();
		WorkingTask = Task.Run(() => CreateSaveForGame(gameResource, gameVersion));
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
	
	/// <summary>
	/// <c>LoadedDatas</c>锁
	/// </summary>
	private static readonly object loadedDatasLock = new();
}
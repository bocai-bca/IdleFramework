using System;
using System.Threading.Tasks;

namespace IdleFramework.Core;

/// <summary>
/// 更新器，对存档数据进行更新的类
/// </summary>
public static class Updater
{
	/// <summary>
	/// 任务结果枚举
	/// </summary>
	public enum WorkResult
	{
		/// <summary>
		/// 工作成功
		/// </summary>
		Success = 0,
		/// <summary>
		/// 存档数据辅助器为null
		/// </summary>
		SaveIsNull = 1,
	}
	
	/// <summary>
	/// 当前是否正在多线程工作(如读写内存中的存档)
	/// </summary>
	public static bool IsMultiThreadWorking => WorkingTask is { IsCompleted: true };

	/// <summary>
	/// 工作线程
	/// </summary>
	public static Task<WorkResult> WorkingTask { get; private set; }

	/// <summary>
	/// 更新器处理的存档数据辅助器。
	/// </summary>
	public static SaveDataHelper SaveDataHelperInHandle { get; set; }
	
	/// <summary>
	/// 更新存档数据的总入口方法，在调用前可能需使用<c>SetDataSafety()</c>将需要更新的<c>SaveData</c>实例传进本类中。
	/// 可能较为耗时，且出于线程安全考虑，建议在所有情况下使用<c>UpdateDataAsync()</c>。本方法主要供<c>Updater</c>工作线程使用。
	/// </summary>
	/// <param name="moveForwardTicks">向前移动的时间刻数。</param>
	/// <returns>任务结果。</returns>
	public static WorkResult UpdateData(long moveForwardTicks) //工作线程方法，不要经过Safety方法而是使用lock直接访问线程保护成员
	{
		if (SaveDataHelperInHandle == null)
		{
			Logger.LogError(Localization.Tr("log.error.updater.save_data_helper_in_handle_is_null"));
			return WorkResult.SaveIsNull;
		}

		return WorkResult.Success;
		bool updateDoneFlag = false;
		while (!updateDoneFlag)
		{
			Guid guidNearestToUpdate;
			
		}
		return WorkResult.Success;
	}

	public static WorkResult UpdateData() //工作线程方法，不要经过Safety方法而是使用lock直接访问线程保护成员
	{
		long moveForwardTicks = TimeHelper.GetUtcNowTick() - SaveAccess.LoadedDataHelper.GetLastUpdateUtcTick();
		return UpdateData(moveForwardTicks);
	}
	
	/// <summary>
	/// 开启工作线程进行存档数据更新。
	/// </summary>
	/// <param name="moveForwardTicks"></param>
	/// <returns></returns>
	public static async Task<WorkResult> UpdateDataAsync(long moveForwardTicks) //含等待方法，请勿在工作线程中使用它
	{
		if (IsMultiThreadWorking) WorkingTask.Wait();
		WorkingTask = Task.Run(() => UpdateData(moveForwardTicks));
		return await WorkingTask;
	}

	public static async Task<WorkResult> UpdateDataAsync() //含等待方法，请勿在工作线程中使用它
	{
		if (IsMultiThreadWorking) WorkingTask.Wait();
		WorkingTask = Task.Run(UpdateData);
		return await WorkingTask;
	}
}
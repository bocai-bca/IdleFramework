using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdleFramework.Global;

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
	/// <param name="updateTargetTicks">更新开始时的时间，按刻数表达，相当于<c>DateTime.Ticks</c>。表示更新前往的目标时间点，也就是让存档从过去更新至现在的这个"现在"。</param>
	/// <returns>任务结果。</returns>
	public static WorkResult UpdateData(long updateTargetTicks) //工作线程方法，不要经过Safety方法而是使用lock直接访问线程保护成员
	{
		if (SaveDataHelperInHandle == null)
		{
			Logger.LogError(Localization.Tr("log.error.updater.save_data_helper_in_handle_is_null"));
			return WorkResult.SaveIsNull;
		}
		InfiniteTaggedValue<long> timeSpanTicksAllowFactoriesToMoveOn = 0L; //在一轮循环中允许每个工厂各自将自身的数据向前推进的时间长度，单位为tick
		InfiniteTaggedValue<long> minimalTimeSpanTicksToNextSomethingChanging; //到达下一状态所需时间最短的对象的所需时间，单位为tick
		bool wasContainerChanged; //记录本轮更新中是否有容器变化，用于控制是否继续循环，还是认为环境热寂而结束循环
		while (true)
		{
			minimalTimeSpanTicksToNextSomethingChanging = long.MaxValue;
			wasContainerChanged = false;
			foreach (string currentSpaceId in SaveDataHelperInHandle.GetAllSpaceIds()) //遍历所有空间ID
			{
				if (!SaveDataHelperInHandle.GetAllInstanceGuidsOfItemsInSpace(currentSpaceId, out Dictionary<string, HashSet<Guid>> itemsInstanceGuids))
				{
					Logger.LogError(string.Format(Localization.Tr("log.error.updater.unexcepted_to_failed_to_get_all_instance_guids_for_items_in_space"), currentSpaceId));
					continue;
				}
				foreach ((string currentItemId, HashSet<Guid> currentItemInstanceGuidsSet) in itemsInstanceGuids)
				{
					if (SaveDataHelperInHandle.UsingGameResource.IsFactory(currentItemId))
					{
						foreach (Guid currentItemInstanceGuid in currentItemInstanceGuidsSet)
						{
							//这里是遍历每个工厂实例的GUID
							if (!SaveDataHelperInHandle.GetFactoryForGuid(currentItemInstanceGuid, out FactoryData currentFactory))
							{
								Logger.LogError(string.Format(Localization.Tr("log.error.updater.unexcepted_to_failed_to_get_factory_for_guid"), currentItemInstanceGuid));
								continue;
							}
							//在这里更新这个工厂，并结合情况修改wasContainerChanged和minimalTimeSpanTickToNextSomethingChanging
							
						}
					}
				}
			}
			if (!wasContainerChanged) break;
			timeSpanTicksAllowFactoriesToMoveOn = minimalTimeSpanTicksToNextSomethingChanging;
		}
		SaveDataHelperInHandle.SetLastUpdateUtcTick(updateTargetTicks);
		return WorkResult.Success;
	}

	public static WorkResult UpdateData() //工作线程方法，不要经过Safety方法而是使用lock直接访问线程保护成员
	{
		return UpdateData(TimeHelper.GetUtcNowTick());
	}
	
	/// <summary>
	/// 开启工作线程进行存档数据更新。
	/// </summary>
	
	/// <returns></returns>
	public static async Task<WorkResult> UpdateDataAsync(long updateTargetTicks) //含等待方法，请勿在工作线程中使用它
	{
		if (IsMultiThreadWorking) WorkingTask.Wait();
		WorkingTask = Task.Run(() => UpdateData(updateTargetTicks));
		return await WorkingTask;
	}

	public static async Task<WorkResult> UpdateDataAsync() //含等待方法，请勿在工作线程中使用它
	{
		if (IsMultiThreadWorking) WorkingTask.Wait();
		WorkingTask = Task.Run(UpdateData);
		return await WorkingTask;
	}
}
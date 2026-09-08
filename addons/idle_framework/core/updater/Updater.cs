using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using IdleFramework.Global;

namespace IdleFramework.Core;

/// <summary>
/// 更新器，对存档数据进行更新的类。由<c>MotherNode</c>管理。
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
	/// 通常由<c>MotherNode</c>赋值，等效于<c>SaveAccess.LoadedDataHelper</c>。
	/// </summary>
	public static SaveDataHelper SaveDataHelperInHandle { get; set; }
	
	/// <summary>
	/// 更新存档数据的总入口方法，在调用前可能需使用<c>SetDataSafety()</c>将需要更新的<c>SaveData</c>实例传进本类中。
	/// 可能较为耗时，且出于线程安全考虑，建议在所有情况下使用<c>UpdateDataAsync()</c>。本方法主要供<c>Updater</c>工作线程使用。
	/// </summary>
	/// <param name="updateTargetTicks">更新开始时的时间，按刻数表达，相当于<c>DateTime.Ticks</c>。表示更新前往的目标时间点，也就是让存档从过去更新至现在的这个"现在"。</param>
	/// <returns>任务结果。</returns>
	public static WorkResult UpdateData(long updateTargetTicks) //工作线程方法
	{
		if (SaveDataHelperInHandle == null)
		{
			Logger.LogError(Localization.Tr("log.error.updater.save_data_helper_in_handle_is_null"));
			return WorkResult.SaveIsNull;
		}
		uint infiniteLoopingTimerWhenTargetTimeReached = uint.MaxValue; //当存档的数据对应时间已到达更新目标时间但由于某些0耗时循环在发生而不断继续存档更新时，本变量充当计时器来限制这种循环迭代的最大次数，超出次数时将强制跳出循环，以免无法终止。
		InfiniteTaggedValue<long> saveTimeCache = SaveDataHelperInHandle.GetLastUpdateUtcTick(); //存档的数据对应时间的缓存，相当于LastUpdateUtcTick的缓存，只在此处读取一次，后续全部用来参与逻辑运算和写入到存档。
		InfiniteTaggedValue<long> timeSpanTicksAllowFactoriesToMoveOn = 0L; //在一轮循环中允许每个工厂各自将自身的数据向前推进的时间长度，单位为tick
		while (true) // 对所有工厂发起遍历
		{
			saveTimeCache = InfiniteTaggedValue<long>.MoveToward(saveTimeCache, updateTargetTicks, timeSpanTicksAllowFactoriesToMoveOn); // 向前推进saveTimeCache
			InfiniteTaggedValue<long> minimalTimeSpanTicksToNextSomethingChanging = long.MaxValue; //到达下一状态所需时间最短的对象的所需时间，将在一轮循环中收集，单位为tick
			bool wasContainerChanged = false; //记录本轮更新中是否有容器变化，用于控制是否继续循环，还是认为环境热寂而结束循环
			foreach (string currentSpaceId in SaveDataHelperInHandle.GetAllSpaceIds()) //遍历所有空间ID
			{
				if (!SaveDataHelperInHandle.GetAllInstanceGuidsOfItemsInSpace(currentSpaceId, out Dictionary<string, HashSet<Guid>> itemsInstanceGuids))
				{
					Logger.LogError(string.Format(Localization.Tr("log.error.updater.unexcepted_to_failed_to_get_all_instance_guids_for_items_in_space"), currentSpaceId));
					continue;
				}
				foreach ((string currentItemId, HashSet<Guid> currentItemInstanceGuidsSet) in itemsInstanceGuids)
				{
					if (!SaveDataHelperInHandle.UsingGameResource.IsFactory(currentItemId)) continue;
					foreach (Guid currentFactoryGuid in currentItemInstanceGuidsSet)
					{
						//这里是遍历每个工厂实例的GUID
						SaveDataHelperInHandle.TryRunRecipeOrderForFactory(currentFactoryGuid, true); // 如果工厂未开始，尝试开始工厂
						if (!SaveDataHelperInHandle.GetFactoryForGuid(currentFactoryGuid, out FactoryData currentFactoryDuplicated))
						{
							Logger.LogError(string.Format(Localization.Tr("log.error.updater.unexcepted_to_failed_to_get_factory_for_guid"), currentFactoryGuid));
							continue;
						}
						if (!currentFactoryDuplicated.WasStarted) // 如果工厂还是未开始，意味着工厂无法开始(如无配方队列等)
						{
							minimalTimeSpanTicksToNextSomethingChanging = 0L; // 设置最小跳过时间
							wasContainerChanged = false;
							continue;
						}
						//在这里更新这个工厂，使用timeSpanTicksAllowFactoriesToMoveOn让工厂推进，并结合情况修改wasContainerChanged和minimalTimeSpanTickToNextSomethingChanging
						wasContainerChanged = updateFactory(currentFactoryDuplicated, timeSpanTicksAllowFactoriesToMoveOn, out InfiniteTaggedValue<long> currentMinimalTimeSpanTicksToNextSomethingChanging) || wasContainerChanged;
						minimalTimeSpanTicksToNextSomethingChanging = InfiniteTaggedValue<long>.Min(minimalTimeSpanTicksToNextSomethingChanging, currentMinimalTimeSpanTicksToNextSomethingChanging);
						SaveDataHelperInHandle.SetInstanceObject(currentFactoryDuplicated, currentFactoryGuid);
					}
				}
			}
			timeSpanTicksAllowFactoriesToMoveOn = minimalTimeSpanTicksToNextSomethingChanging;
			if (wasContainerChanged) continue;
			if (infiniteLoopingTimerWhenTargetTimeReached == 0)
			{
				Logger.LogWarning(Localization.Tr("log.warning.updater.timed_out_on_factory_updating_loop"));
				break;
			}
			if (saveTimeCache == updateTargetTicks) infiniteLoopingTimerWhenTargetTimeReached -= 1;
		}
		SaveDataHelperInHandle.SetLastUpdateUtcTick(updateTargetTicks);
		return WorkResult.Success;
	}

	/// <summary>
	/// 内部专用，更新工厂
	/// </summary>
	/// <param name="factoryData">要更新的工厂数据</param>
	/// <param name="timeSpanTicksAllowFactoriesToMoveOn">允许该工厂向前推进的时间长度，单位为tick。</param>
	/// <param name="minimalTimeSpanTicksToNextSomethingChanging">该工厂到达下一个状态变化(如产出材料)所需的时间长度，单位为tick。</param>
	/// <returns>该工厂是否导致容器发生变化，为true则意味着更新器应当进行下一轮循环。</returns>
	private static bool updateFactory([NotNull]FactoryData factoryData, InfiniteTaggedValue<long> timeSpanTicksAllowFactoriesToMoveOn, out InfiniteTaggedValue<long> minimalTimeSpanTicksToNextSomethingChanging)
	{
		bool containerChanged = false; // 创建局部变量用来记录是否更改过容器
		// 工厂运行和收获
		switch (factoryData.FactoryMode)
		{
			case FactoryIngredientRequireMode.CheckAndConsumeAtStart:
				InfiniteTaggedValue<long> currentRecipeWorkedTime = InfiniteTaggedValue<long>.MoveToward(factoryData.RecipeWorkedTicks, factoryData.RecipeRequiredTicks, timeSpanTicksAllowFactoriesToMoveOn);
				if (currentRecipeWorkedTime >= factoryData.RecipeRequiredTicks)
				{
					//生产完毕，输出产品到容器
					if (!SaveDataHelperInHandle.UsingGameResource.RecipeRegistry.TryGetValue(factoryData.CurrentRecipe, out RecipeRegistryObject recipeRegistryObject))
					{
						Logger.LogError(string.Format(Localization.Tr("log.error.updater.a_factory_data_taking_an_unknown_recipe"), factoryData.CurrentRecipe));
						minimalTimeSpanTicksToNextSomethingChanging = new InfiniteTaggedValue<long>(0L, true);
						return false;
					}
					Dictionary<string, long> itemGoingToAdd = [];
					foreach ((string itemId, NumberProvider numberProvider) in recipeRegistryObject.Results) itemGoingToAdd[itemId] = numberProvider.GetNumber();
					containerChanged = SaveDataHelperInHandle.TryAddItemsForContainer(factoryData.OutputContainerGuid, itemGoingToAdd);
					factoryData.CurrentRecipe = string.Empty;
					factoryData.RecipeWorkedTicks = 0L;
					factoryData.RecipeRequiredTicks = 0L;
				}
				factoryData.RecipeWorkedTicks = currentRecipeWorkedTime.Value;
				minimalTimeSpanTicksToNextSomethingChanging = factoryData.RecipeRemainingTicks;
				return containerChanged;
			// TODO 完成更多工厂模式的更新逻辑
		}
		// 如果到达此处，说明该工厂处于未知的工厂模式，这属于异常状态
		Logger.LogError(Localization.Tr("log.error.updater.a_factory_data_taking_an_unknown_factory_mode"));
		minimalTimeSpanTicksToNextSomethingChanging = 0L;
		return false;
	}

	public static WorkResult UpdateData() //工作线程方法
	{
		return UpdateData(TimeHelper.GetUtcNowTick());
	}
	
	/// <summary>
	/// 开启工作线程进行存档数据更新。
	/// </summary>
	/// <param name="updateTargetTicks">更新开始时的时间，按刻数表达，相当于<c>DateTime.Ticks</c>。表示更新前往的目标时间点，也就是让存档从过去更新至现在的这个"现在"。</param>
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
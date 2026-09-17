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
		/// <summary>
		/// 更新循环次数触及限制，发生为更新过程死循环而无法退出时，循环计数器归零而触发的强制退出。视为一种失败的退出。
		/// </summary>
		UpdateLoopTimesReachedLimit = 2,
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
		// 跨循环变量声明
		ushort loopingEnforceStopTimer = ushort.MaxValue; //循环强制终止计时器，如果一个周期为0推进(无论有没有容器变化)，就使本变量减一，当本变量归零时，强制结束循环。这种结束方式视为游戏资源提供了会导致无限循环的数值设计，是应当予以报错的。
		InfiniteTaggedValue<long> saveTimeCache = SaveDataHelperInHandle.GetLastUpdateUtcTick(); //对存档数据中最后更新时间的缓存，将在此方法内作为更新的缓存，保持参与更新逻辑的演化，并在更新完毕后将最新的时间存储回存档数据中。
		InfiniteTaggedValue<long> timeSpanTicksForMoveInRound = 0L; //每个循环周期允许向前推进的时间，单位为tick。初始值代表了第一个周期允许向前推进的时间，0决定了第一个周期为0推进。此后本变量的值将在每个循环周期中扫描每个工厂并收集其显著可知的下一次导致容器变化的时间。
		// 循环段
		while (true)
		{
			// 防无限循环处理逻辑
			timeSpanTicksForMoveInRound = updateTargetTicks - InfiniteTaggedValue<long>.MoveToward(saveTimeCache, updateTargetTicks, timeSpanTicksForMoveInRound);
			if (timeSpanTicksForMoveInRound == 0L) //如果本次循环是0推进
			{
				loopingEnforceStopTimer -= 1;
				if (loopingEnforceStopTimer == 0)
				{
					SaveDataHelperInHandle.SetLastUpdateUtcTick(saveTimeCache.Value); //将更新到达的时间存储回存档数据中
					Logger.LogWarning(Localization.Tr("log.warning.updater.timed_out_on_factory_updating_loop"));
					return WorkResult.UpdateLoopTimesReachedLimit;
				}
			}
			// 每个循环周期的初始变量声明
			InfiniteTaggedValue<long> minimalTimeSpanTicksForContainerChange = new(long.MaxValue, true); //在本循环周期中遍历所有工厂收集得来的能够致使容器发生变化的最短时间长度，用于确定下一循环周期的向前推进的时间。设置该值时应当遵循思路：假定其他地方不会发生任何变化。
			bool containerChanged = false; //记录本循环周期中是否有容器发生变化，用于决定下一循环周期是否是0推进。
			// 每个循环周期中对所有工厂的遍历
			List<string> allSpaceIds = SaveDataHelperInHandle.GetAllSpaceIds();
			foreach (string currentSpaceId in allSpaceIds) //遍历所有空间ID
			{
				if (!SaveDataHelperInHandle.GetAllInstanceGuidsOfItemsInSpace(currentSpaceId, out Dictionary<string, HashSet<Guid>> itemsInstanceGuids)) //获取该空间中的所有物品实例的GUID
				{
					// 基本上不会到达此处，仅作为后备保险的报错手段。
					Logger.LogError(string.Format(Localization.Tr("log.error.updater.unexcepted_to_failed_to_get_all_instance_guids_for_items_in_space"), currentSpaceId));
					continue;
				}
				foreach ((string currentItemId, HashSet<Guid> currentItemInstanceGuidsSet) in itemsInstanceGuids) //遍历每种物品的[ID,GUID集]键值对
				{
					if (!SaveDataHelperInHandle.UsingGameResource.IsFactory(currentItemId)) continue; //基于物品ID判断，如果这个物品不是工厂就直接跳过。意味着执行到下面的物品都是工厂。
					foreach (Guid currentFactoryGuid in currentItemInstanceGuidsSet) //遍历该工厂的实例的GUID
					{
						SaveDataHelperInHandle.TryRunRecipeOrderForFactory(currentFactoryGuid, true); //如果工厂未开始，尝试开始工厂
						if (!SaveDataHelperInHandle.GetFactoryForGuid(currentFactoryGuid, out FactoryData currentFactoryDuplicated)) //获取该工厂的工厂数据(复制品)
						{
							// 基本上不会到达此处，仅作为后备保险的报错手段。
							Logger.LogError(string.Format(Localization.Tr("log.error.updater.unexcepted_to_failed_to_get_factory_for_guid"), currentFactoryGuid));
							continue;
						}
						if (!currentFactoryDuplicated.WasStarted) //如果工厂还是未开始，意味着工厂无法开始(如无配方队列等)
						{
							continue;
						}
						//在这里更新这个工厂，使用timeSpanTicksAllowFactoriesToMoveOn让工厂推进，并结合情况修改wasContainerChanged和minimalTimeSpanTickToNextSomethingChanging
						containerChanged |= updateFactory(currentFactoryDuplicated, timeSpanTicksForMoveInRound, out InfiniteTaggedValue<long> currentMinimalTimeSpanTicksToNextSomethingChanging);
						minimalTimeSpanTicksForContainerChange = InfiniteTaggedValue<long>.Min(minimalTimeSpanTicksForContainerChange, currentMinimalTimeSpanTicksToNextSomethingChanging);
						SaveDataHelperInHandle.SetInstanceObject(currentFactoryDuplicated, currentFactoryGuid);
					}
				}
			}
			saveTimeCache += timeSpanTicksForMoveInRound; //更新当前的存档时间缓存
			// 遍历完毕后，为下一次循环做准备，或者判断是否要跳出循环。
			if (containerChanged) //如果这轮周期中容器有发生变化
			{
				timeSpanTicksForMoveInRound = 0L; //设置下一轮循环为0推进循环
			}
			else //否则(这轮周期没有容器变化)
			{
				if (saveTimeCache == updateTargetTicks) break; //如果已经到达更新目标时间，则退出循环
				// 这边是如果没有到达目标时间，就要继续循环。这里首先要做准备
				timeSpanTicksForMoveInRound = InfiniteTaggedValue<long>.Min(new InfiniteTaggedValue<long>(long.MaxValue, true), minimalTimeSpanTicksForContainerChange); //设置下一轮循环的允许推进时间
			}
		}
		SaveDataHelperInHandle.SetLastUpdateUtcTick(updateTargetTicks); //将更新到达的时间存储回存档数据中
		return WorkResult.Success;
	}

	/// <summary>
	/// 内部专用，更新工厂
	/// </summary>
	/// <param name="factoryData">要更新的工厂数据</param>
	/// <param name="timeSpanTicksAllowFactoriesToMoveOn">允许该工厂向前推进的时间长度，单位为tick。</param>
	/// <param name="minimalTimeSpanTicksToNextSomethingChanging">该工厂到达下一个状态变化(如产出材料)所需的时间长度，单位为tick。给予值时会遵循默认其他外在始终不会变化为前提，例如若一个工厂没有原料，则它返回的到达下一个状态变化的时间将是无限大。</param>
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
						minimalTimeSpanTicksToNextSomethingChanging = new InfiniteTaggedValue<long>(long.MaxValue, true);
						return false;
					}
					Dictionary<string, long> itemGoingToAdd = [];
					foreach ((string itemId, NumberProvider numberProvider) in recipeRegistryObject.Results) itemGoingToAdd[itemId] = numberProvider.GetNumber();
					containerChanged = SaveDataHelperInHandle.TryAddItemsForContainer(factoryData.OutputContainerGuid, itemGoingToAdd);
					factoryData.CurrentRecipe = string.Empty;
					factoryData.RecipeWorkedTicks = 0L;
					factoryData.RecipeRequiredTicks = 0L;
					minimalTimeSpanTicksToNextSomethingChanging = new InfiniteTaggedValue<long>(long.MaxValue, true);
				}
				else
				{
					factoryData.RecipeWorkedTicks = currentRecipeWorkedTime.Value;
					minimalTimeSpanTicksToNextSomethingChanging = factoryData.RecipeRemainingTicks;
				}
				return containerChanged;
			// TODO 完成更多工厂模式的更新逻辑
		}
		// 如果到达此处，说明该工厂处于未知的工厂模式，这属于异常状态
		Logger.LogError(Localization.Tr("log.error.updater.a_factory_data_taking_an_unknown_factory_mode"));
		minimalTimeSpanTicksToNextSomethingChanging = new InfiniteTaggedValue<long>(long.MaxValue, true);
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
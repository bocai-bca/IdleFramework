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
		Success,
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
	/// 私有<c>SaveData</c>，供<c>Updater</c>内部使用，如需访问请通过本类开放的公共方法
	/// </summary>
	private static SaveData SaveDataInternal { get; set; }

	/// <summary>
	/// 可等待地获取暂存的<c>SaveData</c>实例，如果当前本类的工作线程正在执行，则会阻塞调用方线程。
	/// </summary>
	/// <param name="duplicate">是否复制实例，如果为<c>false</c>则将返回原始实例的引用。为确保多线程安全，建议在所有情况下都保持<c>true</c>来获取复制品实例。</param>
	/// <returns>获取到的<c>SaveData</c>实例</returns>
	public static SaveData GetDataSafety(bool duplicate = true) //工作线程等待方法，请勿在工作线程中使用它
	{
		if (IsMultiThreadWorking) WorkingTask.Wait();
		return duplicate ? SaveDataInternal.Duplicate() : SaveDataInternal;
	}

	/// <summary>
	/// 可等待地设置本类的<c>SaveData</c>暂存实例，如果当前本类的工作线程正在执行，则会阻塞调用方线程。
	/// </summary>
	/// <param name="saveData">要存入的<c>SaveData</c>实例。</param>
	/// <param name="duplicate">是否复制实例，如果为<c>false</c>则将存入原始实例的引用。为确保多线程安全，建议在所有情况下都保持<c>true</c>来获取复制品实例。</param>
	public static void SetDataSafety(SaveData saveData, bool duplicate = true) //工作线程等待方法，请勿在工作线程中使用它
	{
		if (IsMultiThreadWorking) WorkingTask.Wait();
		SaveDataInternal = duplicate ? saveData.Duplicate() : saveData;
	}

	/// <summary>
	/// 更新存档数据，在调用前可能需使用<c>SetDataSafety()</c>将需要更新的<c>SaveData</c>实例传进本类中。
	/// 可能较为耗时，且出于线程安全考虑，建议在所有情况下使用<c>UpdateDataAsync()</c>。本方法主要供<c>Updater</c>工作线程使用。
	/// </summary>
	/// <param name="moveForwardTicks">向前移动的时间刻数。</param>
	/// <returns>任务结果。</returns>
	public static WorkResult UpdateData(long moveForwardTicks) //工作线程方法，不要经过Safety方法而是使用lock直接访问线程保护成员
	{
		// TODO
		return WorkResult.Success;
	}

	public static WorkResult UpdateData() //工作线程方法，不要经过Safety方法而是使用lock直接访问线程保护成员
	{
		long moveForwardTicks;
		lock (saveDataLock)
		{
			moveForwardTicks = TimeHelper.GetUtcNowTick() - SaveDataInternal.LastUpdateUtcTick;
		}
		return UpdateData(moveForwardTicks);
	}

	public static async Task<WorkResult> UpdateDataAsync(long moveForwardTicks) //含等待方法，请勿在工作线程中使用它
	{
		if (IsMultiThreadWorking) WorkingTask.Wait();
		WorkingTask = Task.Run(() => UpdateData(moveForwardTicks));
		return await WorkingTask;
	}
	
	/// <summary>
	/// <c>SaveDataInternal</c>锁
	/// </summary>
	private static readonly object saveDataLock = new();
}
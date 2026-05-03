// MotherNode的扩展文件，用来存放涉及State的各种方法
using System;

namespace IdleFramework.Core;

public partial class MotherNode
{
	public DateTime lastUpdateTime;
	
	private void StateProcess_BeforeLoadSave()
	{
		_ = SaveAccess.LoadLatestSaveForGameAsync(GameResource.GameID);
		CurrentState = State.WaitingForSaveLoading;
	}

	private void StateProcess_WaitingForSaveLoading()
	{
		if (!SaveAccess.WorkingTask.IsCompleted) return; //如果SaveAccess工作线程未完成则离开本帧
		if (!SaveAccess.WorkingTask.IsCompletedSuccessfully) //如果SaveAccess工作线程未成功完成
		{
			AggregateException exception = SaveAccess.WorkingTask.Exception;
			if (exception == null)
			{
				Logger.LogError(Localization.Tr("log.error.mother_node.task_unsuccessfully_without_exception_on_save_access_async"));
				CurrentState = State.FreezeForUnhandlableError;
				return;
			}
			Logger.LogError(string.Format(Localization.Tr("log.error.mother_node.unhandled_exception_on_save_access_async"), exception));
			CurrentState = State.FreezeForUnhandlableError;
			return;
			//报错完离开本帧
		}
		SaveAccess.WorkResult workResult = SaveAccess.WorkingTask.Result;
		switch (workResult)
		{
			case SaveAccess.WorkResult.Success:
				CurrentState = State.MainRunning_WaitingUpdate;
				break;
			case SaveAccess.WorkResult.SaveNotFound:
				_ = SaveAccess.CreateSaveForGameAsync(GameResource, GameResource.GameVersion);
				CurrentState = State.WaitingForSaveLoading; //实际上是个原地跳转，先预留一个赋值，万一以后要在CurrentState赋值器上插什么事件可以用得到
				break;
			case SaveAccess.WorkResult.OtherError:
				Logger.LogError(Localization.Tr("log.error.mother_node.save_access_returned_other_error_of_work_result"));
				CurrentState = State.FreezeForUnhandlableError;
				break;
			case SaveAccess.WorkResult.SaveParsingFailed:
				Logger.LogError(Localization.Tr("log.error.mother_node.save_access_parsing_save_failed"));
				CurrentState = State.FreezeForUnhandlableError;
				break;
			case SaveAccess.WorkResult.FileIOFailed:
				Logger.LogError(Localization.Tr("log.error.mother_node.save_access_errors_on_file_io"));
				CurrentState = State.FreezeForUnhandlableError;
				break;
			case SaveAccess.WorkResult.FindLatestSaveFailed:
				Logger.LogError(Localization.Tr("log.error.mother_node.save_access_failed_to_find_latest_save"));
				CurrentState = State.FreezeForUnhandlableError;
				break;
			default:
				Logger.LogError(Localization.Tr("log.error.mother_node.got_unknown_result_from_save_access_async"));
				CurrentState = State.FreezeForUnhandlableError;
				break;
		}
	}

	// 到达MainRunning时应保证存档已经就绪
	private void StateProcess_MainRunning_WaitingUpdate(double delta)
	{
		
	}
}
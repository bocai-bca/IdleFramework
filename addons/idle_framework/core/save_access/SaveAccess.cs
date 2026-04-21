using System.Collections.Generic;

namespace IdleFramework;

/// <summary>
/// 存档访问，在运行时供UI场景和运行时类读写存档
/// </summary>
public class SaveAccess
{
	/// <summary>
	/// 已加载的数据
	/// </summary>
	public static Dictionary<string, SaveData> LoadedDatas = [];
	
	public static bool LoadSaveForGame(string gameID)
	{
		return false;
	}
}
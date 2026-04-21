using Godot;

namespace IdleFramework;

/// <summary>
/// 存档数据，代表一个游戏的一个存档槽位的一瞬状态数据
/// </summary>
[GlobalClass]
public partial class SaveData : Resource
{
	/// <summary>
	/// 记录本存档对应的游戏ID
	/// </summary>
	public string GameID { get; set; } = "";

	/// <summary>
	/// 记录本存档上一次更新的时间
	/// </summary>
	public long LastUpdateUnixTime { get; set; } = 0L;

	/// <summary>
	/// 从JSON文本解析为<c>SaveData</c>实例
	/// </summary>
	/// <param name="json">待解析的存档数据Json</param>
	/// <returns>解析完毕的<c>SaveData</c>实例</returns>
	public static SaveData ParseFromJSON(string json)
	{
		SaveData result = new SaveData();
		return result;
	}
}
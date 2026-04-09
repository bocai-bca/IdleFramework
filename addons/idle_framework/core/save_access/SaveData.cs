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
	public StringName GameID { get; set; }
	
	/// <summary>
	/// 记录本存档上一次更新的时间
	/// </summary>
	public double LastUpdateUnixTime { get; set; }
}
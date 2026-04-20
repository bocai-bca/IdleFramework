using Godot;

namespace IdleFramework;

/// <summary>
/// 数值提供器-范围随机
/// 指定一个范围，每次从中返回一个随机数字，随机平均分布
/// </summary>
[GlobalClass]
public partial class NumberProviderRange : NumberProvider
{
	/// <summary>
	/// 最小值(含)
	/// </summary>
	[Export]
	[ExportGroup("Data")]
	public int MinNumber { get; set; }
	
	/// <summary>
	/// 最大值(含)
	/// </summary>
	[Export]
	public int MaxNumber { get; set; }

	/// <summary>
	/// GetValue()的实现
	/// </summary>
	/// <returns>返回Value属性的值</returns>
	public override int GetNumber()
	{
		return GD.RandRange(MinNumber, MaxNumber);
	}
}
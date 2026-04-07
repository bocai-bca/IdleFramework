using Godot;

namespace IdleFramework;

/// <summary>
/// 数值提供器-立即数
/// 指定一个固定的数字，每次都返回固定的数字
/// </summary>
[GlobalClass]
public partial class NumberProviderDirect : NumberProvider
{
	/// <summary>
	/// 该立即数提供器会返回的值
	/// </summary>
	[Export]
	public int Number { get; set; }

	/// <summary>
	/// GetValue()的实现
	/// </summary>
	/// <returns>返回Value属性的值</returns>
	public override int GetNumber()
	{
		return Number;
	}
}
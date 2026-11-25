using Godot;

namespace IdleFramework.Core;

/// <summary>
/// 数值提供器-壹
/// 固定返回1。
/// </summary>
[GlobalClass]
public partial class NumberProviderOne : NumberProvider
{
	public override long GetNumber() => 1L;
	public override long NumberPreviewAvg => 1L;
	public override string NumberPreviewText => "1";
}
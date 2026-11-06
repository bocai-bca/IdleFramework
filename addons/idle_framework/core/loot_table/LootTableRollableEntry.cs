using Godot;

namespace IdleFramework;

/// <summary>
/// 掉落表可抽取的项目。
/// </summary>
[GlobalClass]
public partial class LootTableRollableEntry: Resource
{
	/// <summary>
	/// 权重，数值越高越容易被抽取到。最小要达到1，权重为0或负数的项不会被抽到。
	/// 每次抽取抽取时都会获取一次权重，因此权重可以在多轮抽取中变化。
	/// </summary>
	[Export] 
	public NumberProvider Weight { get; set; }
		
	/// <summary>
	/// 该池项目的掉落表。
	/// </summary>
	[Export]
	public LootTable LootTable { get; set; }
		
	/// <summary>
	/// 是否允许本项被重复抽取，如果为<c>false</c>，则在同一次抽取的不同轮数中不会被再次抽到。如果池中所有项都没抽到过，即使轮数还未用尽也将结束抽取。
	/// </summary>
	[Export]
	public bool AllowDuplicates { get; set; } = true;
}
using Godot;
using Godot.Collections;

namespace IdleFramework;

/// <summary>
/// 配方下单器-列表随机
/// 拉取时随机从配方ID列表中选取一个元素
/// </summary>
[GlobalClass]
public partial class RecipeOrderListRandom : RecipeOrder
{
	/// <summary>
	/// 本下单器的配方ID列表，允许重复出现相同ID，重复ID会表现为提高该ID被抽中的概率
	/// </summary>
	[Export]
	[ExportGroup("Data")]
	public Array<StringName> RecipeIDs { get; set; } = [];

	/// <summary>
	/// 拉取配方，返回RecipeIDs中随机抽取的一项
	/// </summary>
	/// <returns>本下单器实例提供的配方</returns>
	public override StringName PullRecipe()
	{
		return RecipeIDs[GD.RandRange(0, RecipeIDs.Count - 1)];
	}
}
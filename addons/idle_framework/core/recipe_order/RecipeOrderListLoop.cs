using Godot;
using Godot.Collections;

namespace IdleFramework;

/// <summary>
/// 配方下单器-列表循环
/// 拉取时转动索引记录，使每次拉取后下一次拉取都为配方ID列表中的下一个元素，到达尽头后回到开头
/// </summary>
[GlobalClass]
public partial class RecipeOrderListLoop : RecipeOrder
{
	/// <summary>
	/// 本下单器的配方ID列表，允许重复出现相同ID
	/// </summary>
	[Export]
	[ExportGroup("Data")]
	public Array<string> RecipeIDs { get; set; } = [];

	/// <summary>
	/// 本下单器的索引记录
	/// </summary>
	public int indexCounter;
	
	/// <summary>
	/// 拉取配方，按顺序依次返回RecipeIDs
	/// </summary>
	/// <returns>本下单器实例提供的配方</returns>
	public override string PullRecipe()
	{
		string result = RecipeIDs[indexCounter];
		indexCounter = (indexCounter + 1) % RecipeIDs.Count;
		return result;
	}
}
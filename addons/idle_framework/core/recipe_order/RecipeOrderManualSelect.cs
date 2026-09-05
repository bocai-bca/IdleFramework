using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace IdleFramework;

/// <summary>
/// 配方下单器-手动选择
/// 手动选择下单器是一种依靠玩家主动操作的下单器，拉取它时如果队列没有等待配方则不会返回配方(返回空字符串)，并在队列添加首个配方时推送配方
/// </summary>
[GlobalClass]
public partial class RecipeOrderManualSelect: RecipeOrderStorable
{
	public override bool IsManuallable => true;
	
	/// <summary>
	/// 本下单器的配方ID列表，会呈现在GUI上供玩家手动选择，因此不要重复出现相同ID。
	/// 赋值器是一个后备访问接口，请勿轻易使用。
	/// </summary>
	[Export]
	[ExportGroup("Data")]
	public Array<string> RecipeIDs { get; set; } = [];

	/// <summary>
	/// 拉取配方，返回队列中排在最前的一个配方ID，如果队列为空则返回空字符串。
	/// </summary>
	/// <returns>本下单器实例提供的配方，若队列为空则返回<c>string.Empty</c>。</returns>
	public override string PullRecipe(List<long> argumentsLong, Queue<string> queueString)
	{
		return queueString.Count == 0 ? string.Empty : queueString.Dequeue();
	}
}
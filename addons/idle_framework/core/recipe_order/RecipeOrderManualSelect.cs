using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace IdleFramework;

/// <summary>
/// 配方下单器-手动选择
/// 手动选择下单器是一种依靠玩家主动操作的下单器，拉取它时如果队列没有等待配方则不会返回配方(返回空字符串)，并在队列添加首个配方时推送配方
/// </summary>
[GlobalClass]
public partial class RecipeOrderManualSelect : RecipeOrder
{
	/// <summary>
	/// 本下单器的配方ID列表，会呈现在GUI上供玩家手动选择，因此不要重复出现相同ID
	/// </summary>
	[Export] public Array<StringName> RecipeIDs { get; set; } = [];
	
	/// <summary>
	/// 本下单器可以暂存的配方队列容量，单位为配方个数，如果给定数字小于1则无法添加配方(但可以在队列为空时推送配方)
	/// 本下单器可能在很多时候从本数值提供器获取值，因此建议不要使用太复杂的数值提供器，也尤其避免制作可能造成无限递归的数值提供器链路
	/// </summary>
	[Export] public NumberProvider QueueSize;

	/// <summary>
	/// 检查本下单器的队列是否已满，在访问本属性时会调用QueueSize的数值提供器的GetNumber()方法
	/// </summary>
	public bool IsFull => RecipeQueue.Count >= QueueSize.GetNumber();
	
	/// <summary>
	/// 本下单器的队列，类型使用System.Collections.Generic.Queue
	/// </summary>
	public Queue<StringName> RecipeQueue = new();
	
	/// <summary>
	/// 拉取配方，返回队列中的下一个配方或者返回空字符串
	/// </summary>
	/// <returns>本下单器实例提供的配方</returns>
	public override StringName PullRecipe()
	{
		if (RecipeQueue.TryDequeue(out StringName recipe))
		{
			return recipe;
		}
		return "";
	}
	
	/// <summary>
	/// 添加配方到队列
	/// </summary>
	/// <param name="recipe">要添加的配方</param>
	/// <returns>成功与否(在队列装满时丢弃并返回false)</returns>
	public bool EnqueueRecipe(StringName recipe)
	{
		if (IsFull) return false;
		if (RecipeQueue.Count == 0)
		{
			RecipeQueue.Enqueue(recipe);
			PushRecipe();
			return true;
		}
		RecipeQueue.Enqueue(recipe);
		return true;
	}

	/// <summary>
	/// 推送配方的覆写
	/// </summary>
	public override void PushRecipe()
	{
		if (RecipeQueue.TryDequeue(out StringName recipe))
		{
			EmitSignal(RecipeOrder.SignalName.RecipePush, recipe);
		}
	}
}
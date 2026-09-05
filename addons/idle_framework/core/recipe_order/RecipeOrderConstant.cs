using System.Collections.Generic;
using Godot;

namespace IdleFramework;

/// <summary>
/// 配方下单器-常量
/// 在任何时候拉取都会返回于RecipeID属性指定的配方ID(如果使用脚本修改RecipeID属性，则会改变该实例返回的配方ID)
/// </summary>
[GlobalClass]
public partial class RecipeOrderConstant : RecipeOrder
{
	public override bool IsManuallable => false;

	/// <summary>
	/// 本下单器会返回的配方ID。
	/// 赋值器是一个后备访问接口，请勿轻易使用。
	/// </summary>
	[Export]
	[ExportGroup("Data")]
	public string RecipeID { get; set; } = "";
	
	/// <summary>
	/// 拉取配方，返回固定的配方ID。
	/// </summary>
	/// <returns>本下单器实例提供的配方。</returns>
	public override string PullRecipe(List<long> argumentsLong, Queue<string> queueString)
	{
		return RecipeID;
	}
}
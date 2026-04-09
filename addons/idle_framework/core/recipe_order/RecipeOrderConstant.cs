using Godot;

namespace IdleFramework;

/// <summary>
/// 配方下单器-常量
/// 在任何时候拉取都会返回于RecipeID属性指定的配方ID(如果使用脚本修改RecipeID属性，则会改变该实例返回的配方ID)
/// </summary>
[GlobalClass]
public partial class RecipeOrderConstant : RecipeOrder
{
	/// <summary>
	/// 本下单器会返回的配方ID
	/// </summary>
	[Export]
	public StringName RecipeID { get; set; } = "";
	
	/// <summary>
	/// 拉取配方，固定返回RecipeID属性的值
	/// </summary>
	/// <returns>本下单器实例提供的配方</returns>
	public override StringName PullRecipe()
	{
		return RecipeID;
	}
}
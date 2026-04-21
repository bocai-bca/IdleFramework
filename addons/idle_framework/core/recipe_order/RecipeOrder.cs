using Godot;

namespace IdleFramework;

/// <summary>
/// 配方下单器的抽象基类
/// </summary>
[GlobalClass]
public abstract partial class RecipeOrder : Resource
{
	/// <summary>
	/// 拉取配方订单的抽象方法，需要在子类中实现
	/// </summary>
	/// <returns>该配方下单器本次获取提供的配方ID</returns>
	public abstract string PullRecipe();
	
	/// <summary>
	/// 订单推送信号，由工厂实例连接此信号，其参数为此次信号推送上去的配方
	/// </summary>
	[Signal]
	public delegate void RecipePushEventHandler(string recipeID);

	/// <summary>
	/// 推送配方订单，调用此方法会使下单器向自己拉取一次配方并将结果通过信号广播出。对于高级需求，也可以覆写本方法，在实现中也依然要通过广播信号的方式推送配方
	/// </summary>
	public virtual void PushRecipe()
	{
		EmitSignal(SignalName.RecipePush, PullRecipe());
	}
}
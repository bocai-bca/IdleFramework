using Godot;

namespace IdleFramework.Global;

/// <summary>
/// 可提供ClassPackedScene的接口
/// </summary>
public interface IClassPackedScene
{
	/// <summary>
	/// ClassPackedScene(类型场景封包)，请在继承类型中实现此自动属性的取值器，使其返回一个代表该类型的场景的PackedScene
	/// </summary>
	/// <example><code>public partial class ExampleScene : Node , IClassPackedScene
	/// {
	///	  public PackedScene CPS => field ??= GD.Load&lt;PackedScene&gt;("res://example_scene.tscn");
	/// }</code></example>
	public static abstract PackedScene CPS { get; }
}

/// <summary>
/// ClassPackedScene扩展
/// </summary>
public static class ClassPackedSceneExtension
{
	/// <typeparam name="T">继承自IClassPackedScene的类型</typeparam>
	extension<T>(T) where T : class, IClassPackedScene
	{
		/// <summary>
		/// 创建并获取一个该类型的场景实例
		/// </summary>
		/// <returns>该类型的场景或null</returns>
		/// <example><code>public partial class ExampleNode : Node
		/// {
		///   public override void _Ready()
		///   {
		///     AddChild(ExampleScene.Create());
		///   }
		/// }</code></example>
		public static T Create()
		{
			return T.CPS.Instantiate<T>();
		}
	}
}
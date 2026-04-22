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
	///	  public PackedScene CPS
	///	  {
	///	    get
	///	    {
	///	      field ??= GD.Load&lt;PackedScene&gt;("res://example_scene.tscn");
	///	      return field;
	///	    }
	///   }
	/// }</code></example>
	public PackedScene CPS { get; }
}
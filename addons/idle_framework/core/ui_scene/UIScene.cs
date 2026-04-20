using Godot;

namespace IdleFramework;

/// <summary>
/// UI场景基类
/// </summary>
[GlobalClass]
public abstract partial class UIScene : Node
{
	/// <summary>
	/// 对主节点的引用，用于供UI场景自己访问主节点，由主节点在实例化UI场景后赋值
	/// </summary>
	public MotherNode MotherNode { get; set; }
}
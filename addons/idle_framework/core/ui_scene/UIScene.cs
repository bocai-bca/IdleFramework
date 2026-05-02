using Godot;

namespace IdleFramework.Core;

/// <summary>
/// UI场景基类
/// </summary>
[GlobalClass]
public abstract partial class UIScene : Node
{
	/// <summary>
	/// 对主节点的引用，用于供UI场景自己访问主节点，由主节点在实例化UI场景后赋值，可以在_EnterTree()时及之后使用
	/// </summary>
	public MotherNode MotherNodeReference { get; set; }
	
	/// <summary>
	/// 当主节点通知UI场景可以读取游戏资源以构建界面时会调用本方法
	/// </summary>
	public virtual void OnGameResourceReady()
	{
	}
	
	/// <summary>
	/// 当主节点开始异步加载存档时会调用本方法
	/// </summary>
	public virtual void OnSaveStartLoading()
	{
	}

	/// <summary>
	/// 当主节点异步加载存档完成时会调用本方法
	/// </summary>
	public virtual void OnSaveLoaded()
	{
	}

	/// <summary>
	/// 当主节点异步加载存档出错时会调用本方法，此时不会调用<c>OnSaveLoaded()</c>
	/// </summary>
	public virtual void OnSaveLoadFailed()
	{
	}

}
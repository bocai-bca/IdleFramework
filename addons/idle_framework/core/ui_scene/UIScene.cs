using Godot;
using Godot.Collections;

namespace IdleFramework.Core;

/// <summary>
/// UI场景基类
/// </summary>
[GlobalClass]
public abstract partial class UIScene : Node
{
	/// <summary>
	/// 使主窗口的最小尺寸设置为本属性返回的尺寸，为抽象属性，需要在子类中实现。
	/// </summary>
	public abstract Vector2I WindowMinSize { get; }

	/// <summary>
	/// UI场景要添加到翻译域的翻译表，为抽象属性，需要在子类中实现。
	/// </summary>
	public abstract Array<Translation> UISceneTranslations { get; }
	
	/// <summary>
	/// 当主节点通知UI场景可以读取游戏资源以构建界面时会调用本方法
	/// </summary>
	public virtual void OnGameResourceReady()
	{
	}
	
	/// <summary>
	/// 当主节点开始存档访问异步加载存档时会调用本方法
	/// </summary>
	public virtual void OnSaveStartLoading()
	{
	}

	/// <summary>
	/// 当主节点检查到存档访问异步加载存档完成时会调用本方法
	/// </summary>
	public virtual void OnSaveLoaded()
	{
	}

	/// <summary>
	/// 当主节点检查到存档访问异步加载存档出错时会调用本方法，此时不会调用<c>OnSaveLoaded()</c>
	/// </summary>
	public virtual void OnSaveLoadFailed()
	{
	}

	/// <summary>
	/// 当主节点检查到更新器完成一次更新时会调用本方法。
	/// </summary>
	/// <param name="saveDataHelper">允许UI场景访问的存档数据辅助器。</param>
	public virtual void OnUpdaterDone(SaveDataHelper saveDataHelper)
	{
	}

}
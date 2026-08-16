#if IDLE_FRAMEWORK_UISCENE_ALL || IDLE_FRAMEWORK_UISCENE_CONTROL
using Godot;

namespace IdleFramework.UIScenes.Control;

/// <summary>
/// 弹窗标签页的基类，所有用于<c>TabPopup</c>的弹窗节点，必须都继承自此类。
/// </summary>
[GlobalClass]
public abstract partial class PopupTabBase: PanelContainer
{
	/// <summary>
	/// 标签页自己想要关闭时发出，用于告知父级节点。
	/// </summary>
	[Signal]
	public delegate void CloseEventHandler();

	/// <summary>
	/// 标签页的名称，为面向<c>TabPopup</c>的管理名称，用作标签页的唯一标识名，会在添加到<c>TabPopup</c>时被其赋值。
	/// </summary>
	public string TabName { get; set; }
}
#endif
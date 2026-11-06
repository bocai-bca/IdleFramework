using System.Collections.Generic;
using Godot;

namespace IdleFramework;

/// <summary>
/// 掉落表的抽象基类
/// </summary>
[GlobalClass]
public abstract partial class LootTable: Resource
{
	/// <summary>
	/// 获取掉落物的抽象方法，需要在子类中实现
	/// </summary>
	/// <returns>该掉落表本次获取提供的掉落物，键代表物品ID，值代表物品数量。</returns>
	public abstract Dictionary<string, long> GetLoot();
	
	/// <summary>
	/// 递归锁，在部分掉落表中有效，用以防止无限循环引用。
	/// </summary>
	protected bool recursionLock;
}
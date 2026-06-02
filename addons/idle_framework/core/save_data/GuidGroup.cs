using System;
using System.Collections.Generic;

namespace IdleFramework.Core;

/// <summary>
/// GUID编组，一种实例对象。表示一群不重复的GUID。
/// </summary>
public class GuidGroup
{
	/// <summary>
	/// 该GUID编组的存储集合
	/// </summary>
	public HashSet<Guid> Guids { get; } = [];

	public HashSet<Guid>.Enumerator GetEnumerator() => Guids.GetEnumerator();
	public bool Add(Guid guid) => Guids.Add(guid);
	public bool Remove(Guid guid) => Guids.Remove(guid);
	public bool Contains(Guid guid) => Guids.Contains(guid);
}
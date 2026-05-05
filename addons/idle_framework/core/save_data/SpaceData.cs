using System;
using Newtonsoft.Json.Linq;

namespace IdleFramework.Core;

/// <summary>
/// 空间数据，于存档数据中作为单个空间的数据存储
/// </summary>
public class SpaceData : ISaveDataComponent<SpaceData>
{
	/// <summary>
	/// 空间容器GUID
	/// </summary>
	public Guid SpaceContainerGuid { get; set; }
	
	/// <summary>
	/// 转换到Json。
	/// </summary>
	/// <returns>转换后的<c>JToken</c>，实际上是<c>JObject</c>。</returns>
	public JToken ToJson()
	{
		JObject jObject = new();
		JValue jValue = new(SpaceContainerGuid.ToString());
		jObject[nameof(SpaceContainerGuid)] = jValue;
		return jObject;
	}

	/// <summary>
	/// 从Json解析。
	/// </summary>
	/// <param name="jObject">要解析的Json对象。</param>
	/// <returns>解析完成的<c>SpaceData</c>实例，或者失败时返回<c>null</c>。</returns>
	public static SpaceData FromJson(JObject jObject)
	{
		if (jObject == null) return null;
		SpaceData result = new();
		if (jObject.Value<string>(nameof(SpaceContainerGuid)) is { } valueSpaceContainerGuid && Guid.TryParse(valueSpaceContainerGuid.ToString(), out Guid parsedGuid)) result.SpaceContainerGuid = parsedGuid;
		return result;
	}

	/// <summary>
	/// 复制该空间数据实例。
	/// </summary>
	/// <returns>复制出来的新实例。</returns>
	public SpaceData Duplicate()
	{
		SpaceData duplicated = new()
		{
			SpaceContainerGuid = SpaceContainerGuid,
		};
		return duplicated;
	}
}
using Newtonsoft.Json.Linq;

namespace IdleFramework.Core;

/// <summary>
/// 存档数据组件接口，所有存档数据组件应当继承此接口
/// </summary>
/// <typeparam name="T">存档数据组件类型形参，应当将继承此接口的类型自己作为实参填入此处</typeparam>
public interface ISaveDataComponent<T> where T : ISaveDataComponent<T>
{
	
	/// <summary>
	/// 将该存档数据组件转换到<c>JToken</c>。
	/// </summary>
	/// <returns>转换后的<c>JToken</c>。</returns>
	public JToken ToJson();
	
	/// <summary>
	/// 从<c>JToken</c>解析为本存档数据组件类型并返回解析完成的实例。
	/// </summary>
	/// <param name="jToken">待解析的<c>JToken</c></param>
	/// <returns>解析完成的实例，如果失败则返回null，或者其他行为。</returns>
	public static abstract T FromJson(JToken jToken);

	/// <summary>
	/// 复制该存档数据组件实例，如果有引用其他存档数据组件则应当深度递归复制。
	/// </summary>
	/// <returns>复制出来的新实例。</returns>
	public T Duplicate();
}
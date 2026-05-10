namespace IdleFramework.Core;

/// <summary>
/// 工厂原料需求模式，控制工厂对原料的需求逻辑。
/// </summary>
public enum FactoryIngredientRequireMode
{
	/// <summary>
	/// [开始]--过程--结束，开始不满足时无法开始，一旦开始必定成功。
	/// 在开始时检查并消耗材料，成功消耗需求材料才会开始生产，生产完毕后直接产出产品。
	/// </summary>
	CheckAndConsumeAtStart,
	/// <summary>
	/// (开始)--过程--[结束]，结束不满足时等待。
	/// 在开始时检查库存材料是否满足，满足才允许开始生产，在生产完毕时尝试消耗材料，如果材料不足够则保持生产进度并等待，直到成功消耗材料时产出产品。
	/// </summary>
	CheckAtStartAndConsumeAtEnd,
	/// <summary>
	/// 开始--(过程)--[结束]，过程不满足时暂停。
	/// 可直接开始配方，在生产过程中持续检查材料是否满足，如果不满足则暂停进度，在生产完毕时尝试消耗材料，成功消耗材料则产出产品。
	/// </summary>
	CheckToPauseOnProcessAndConsumeAtEnd,
	/// <summary>
	/// 开始--(过程)--[结束]，过程不满足时重置。
	/// 可直接开始配方，在生产过程中持续检查材料是否满足，如果不满足则重置进度，在生产完毕时尝试消耗材料，成功消耗材料则产出产品。
	/// </summary>
	CheckToRestartOnProcessAndConsumeAtEnd,
	/// <summary>
	/// 开始--过程--[结束]，结束不满足时等待。
	/// 可直接开始配方，生产完毕时尝试消耗材料，如果材料不足则保持生产进度并等待，直到成功消耗材料时产出产品。
	/// </summary>
	CheckAndConsumeAtEndAsWait,
	/// <summary>
	/// 开始--过程--[结束]，结束不满足时重置。
	/// 可直接开始配方，生产完毕时尝试消耗材料，如果材料不满足则重置进度，或者成功消耗材料则产出产品。
	/// </summary>
	CheckAndConsumeAtEndAsRestart,
}
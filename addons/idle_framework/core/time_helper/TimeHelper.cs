using System;

namespace IdleFramework.Core;

/// <summary>
/// 提供时间相关的辅助类，封装所有计算方式为命名清晰的方法，以便降低处理时间相关逻辑时的头脑负担
/// </summary>
public class TimeHelper
{
    /// <summary>
    /// 获取当前UTC时间，单位为Tick(100纳秒)
    /// </summary>
    /// <returns>当前的Tick数</returns>
    public static long GetUtcNowTick() => DateTime.UtcNow.Ticks;

    /// <summary>
    /// 获取当前UTC时间，单位为毫秒
    /// </summary>
    /// <returns>当前的毫秒数</returns>
    public static long GetUtcNowMsec() => DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
    
    /// <summary>
    /// 从给定Tick数转换时间到毫秒
    /// </summary>
    /// <returns>转换的毫秒数</returns>
    public static long GetUtcNowMsec(long ticks) => ticks / TimeSpan.TicksPerMillisecond;
    
    /// <summary>
    /// 获取当前UTC时间，单位为秒
    /// </summary>
    /// <returns>当前的秒数</returns>
    public static long GetUtcNowSec() => DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond;
    
    /// <summary>
    /// 从给定Tick数转换时间到秒
    /// </summary>
    /// <returns>转换的秒数</returns>
    public static long GetUtcNowSec(long ticks) => ticks / TimeSpan.TicksPerSecond;
}
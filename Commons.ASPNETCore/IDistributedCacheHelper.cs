using Microsoft.Extensions.Caching.Distributed;

namespace Commons.ASPNETCore;

/// <summary>
/// 分布式缓存帮助类接口，用于获取或创建指定缓存键的值，并设置过期时间为expireSeconds至两倍expireSeconds之间的随机偏移量
/// </summary>
public interface IDistributedCacheHelper
{
    /// <summary>
    /// 获取或创建指定缓存键的值
    /// </summary>
    /// <typeparam name="TItem">值的类型</typeparam>
    /// <param name="cacheKey">缓存键</param>
    /// <param name="valueFactory">值的创建方法</param>
    /// <param name="expireSeconds">缓存过期时间（以秒为单位），默认为60秒</param>
    /// <returns>缓存值</returns>
    TItem? GetOrCreate<TItem>(string cacheKey, Func<DistributedCacheEntryOptions, TItem> valueFactory, int expireSeconds = 60);

    /// <summary>
    /// 异步获取或创建指定缓存键的值
    /// </summary>
    /// <typeparam name="TItem">值的类型</typeparam>
    /// <param name="cacheKey">缓存键</param>
    /// <param name="valueFactory">值的创建方法</param>
    /// <param name="expireSeconds">缓存过期时间（以秒为单位），默认为60秒</param>
    /// <returns>缓存值</returns>
    Task<TItem?> GetOrCreateAsync<TItem>(string cacheKey, Func<DistributedCacheEntryOptions, Task<TItem>> valueFactory, int expireSeconds = 60);

    /// <summary>
    /// 移除指定缓存键的值
    /// </summary>
    /// <param name="cacheKey">缓存键</param>
    void Remove(string cacheKey);

    /// <summary>
    /// 异步移除指定缓存键的值
    /// </summary>
    /// <param name="cacheKey">缓存键</param>
    Task RemoveAsync(string cacheKey);
}

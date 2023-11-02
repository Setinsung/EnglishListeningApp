using Microsoft.Extensions.Caching.Memory;

namespace Commons.ASPNETCore;

/// <summary>
/// 内存缓存帮助类接口，验证不允许是IEnumerable和IQueryable，避开延迟加载。同时设置过期时间为expireSeconds至两倍expireSeconds之间随机偏移
/// </summary>
public interface IMemoryCacheHelper
{
    /// <summary>
    /// 获取或创建指定缓存键的值
    /// </summary>
    /// <typeparam name="TItem">值的类型</typeparam>
    /// <param name="cacheKey">缓存键</param>
    /// <param name="valueFactory">值的创建方法</param>
    /// <param name="expireSeconds">缓存过期时间（以秒为单位），默认为60秒</param>
    /// <returns>缓存值</returns>
    TItem? GetOrCreate<TItem>(string cacheKey, Func<ICacheEntry, TItem> valueFactory, int expireSeconds = 60);

    /// <summary>
    /// 异步获取或创建指定缓存键的值
    /// </summary>
    /// <typeparam name="TItem">值的类型</typeparam>
    /// <param name="cacheKey">缓存键</param>
    /// <param name="valueFactory">值的创建方法</param>
    /// <param name="expireSeconds">缓存过期时间（以秒为单位），默认为60秒</param>
    /// <returns>缓存值</returns>
    Task<TItem?> GetOrCreateAsync<TItem>(string cacheKey, Func<ICacheEntry, Task<TItem>> valueFactory, int expireSeconds = 60);


    /// <summary>
    /// 移除指定缓存键的值
    /// </summary>
    /// <param name="cacheKey">缓存键</param>
    void Remove(string cacheKey);
}

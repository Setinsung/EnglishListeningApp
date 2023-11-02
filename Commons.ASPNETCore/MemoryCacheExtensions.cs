using Microsoft.Extensions.Caching.Memory;

namespace Commons.ASPNETCore;

public static class MemoryCacheExtensions
{
    /// <summary>
    /// 获取或创建指定缓存键的值。验证不允许是IEnumerable和IQueryable，避开延迟加载。同时设置过期时间为expireSeconds至两倍expireSeconds之间随机偏移。
    /// </summary>
    /// <typeparam name="TItem">值的类型</typeparam>
    /// <param name="cacheKey">缓存键</param>
    /// <param name="valueFactory">值的创建方法</param>
    /// <param name="expireSeconds">缓存过期时间（以秒为单位），默认为60秒</param>
    /// <returns>缓存值</returns>
    public static TItem? GetOrCreate<TItem>(this IMemoryCache memoryCache, string cacheKey, Func<ICacheEntry, TItem> valueFactory, int expireSeconds = 60)
    {
        ValidateValueType<TItem>();
        if (!memoryCache.TryGetValue(cacheKey, out TItem? result))
        {
            using ICacheEntry entry = memoryCache.CreateEntry(cacheKey);
            InitCacheEntry(entry, expireSeconds);
            result = valueFactory(entry);
            entry.Value = result;
        }
        return result;
    }

    /// <summary>
    /// 异步获取或创建指定缓存键的值。验证不允许是IEnumerable和IQueryable，避开延迟加载。同时设置过期时间为expireSeconds至两倍expireSeconds之间随机偏移。
    /// </summary>
    /// <typeparam name="TItem">值的类型</typeparam>
    /// <param name="cacheKey">缓存键</param>
    /// <param name="valueFactory">值的创建方法</param>
    /// <param name="expireSeconds">缓存过期时间（以秒为单位），默认为60秒</param>
    /// <returns>缓存值</returns>
    public static async Task<TItem?> GetOrCreateAsync<TItem>(this IMemoryCache memoryCache, string cacheKey, Func<ICacheEntry, Task<TItem>> valueFactory, int expireSeconds = 60)
    {
        ValidateValueType<TItem>();
        if (!memoryCache.TryGetValue(cacheKey, out TItem? result))
        {
            using ICacheEntry entry = memoryCache.CreateEntry(cacheKey);
            InitCacheEntry(entry, expireSeconds);
            result = await valueFactory(entry).ConfigureAwait(false);
            entry.Value = result;
        }
        return result;
    }

    /// <summary>
    /// 验证类型。不允许是IEnumerable和IQueryable，避开延迟加载。
    /// </summary>
    private static void ValidateValueType<T>()
    {
        Type typeResult = typeof(T);
        if (typeResult.IsGenericType)
        {
            typeResult = typeResult.GetGenericTypeDefinition(); // 获取泛型定义类型
        }
        if (typeResult == typeof(IEnumerable<>) ||
            typeResult == typeof(IAsyncEnumerable<>) ||
            typeResult == typeof(IQueryable<>))
        {
            throw new InvalidOperationException($"T of {typeResult} is not allowed, please use List<T> or T[] instead.");
        }
    }
    /// <summary>
    /// 初始化缓存条目entry，设置过期时间为expireSeconds至两倍expireSeconds之间随机偏移。
    /// </summary>
    /// <param name="entry">缓存条目。</param>
    /// <param name="expireSeconds">过期时间（秒）。</param>
    private static void InitCacheEntry(ICacheEntry entry, int expireSeconds)
    {
        double sec = Random.Shared.NextDouble(expireSeconds, expireSeconds * 2);
        TimeSpan expire = TimeSpan.FromSeconds(sec);
        entry.AbsoluteExpirationRelativeToNow = expire;
    }
}

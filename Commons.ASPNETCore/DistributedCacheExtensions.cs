using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Commons.ASPNETCore;

public static class DistributedCacheExtensions
{
    /// <summary>
    /// 获取或创建指定缓存键的值。验证不允许是IEnumerable和IQueryable，避开延迟加载。同时设置过期时间为expireSeconds至两倍expireSeconds之间随机偏移。滑动刷新过期时间。
    /// </summary>
    /// <typeparam name="TItem">值的类型</typeparam>
    /// <param name="cacheKey">缓存键</param>
    /// <param name="valueFactory">值的创建方法</param>
    /// <param name="expireSeconds">缓存过期时间（以秒为单位），默认为60秒</param>
    /// <returns>缓存值</returns>
    public static TItem? GetOrCreate<TItem>(this IDistributedCache distributedCache, string cacheKey, Func<DistributedCacheEntryOptions, TItem> valueFactory, int expireSeconds = 60)
    {
        string? jsonResult = distributedCache.GetString(cacheKey);
        if (jsonResult == null)
        {
            ValidateValueType<TItem>();
            var options = InitDistributedCacheEntryOptions(expireSeconds);
            TItem? result = valueFactory(options); // 数据源存储的null值会被json序列化为字符串"null"
            jsonResult = JsonSerializer.Serialize(result, typeof(TItem));
            distributedCache.SetString(cacheKey, jsonResult, options);
            return result;
        }
        else
        {
            distributedCache.Refresh(cacheKey); // 刷新滑动过期时间
            return JsonSerializer.Deserialize<TItem>(jsonResult);
        }
    }

    /// <summary>
    /// 异步获取或创建指定缓存键的值。验证不允许是IEnumerable和IQueryable，避开延迟加载。同时设置过期时间为expireSeconds至两倍expireSeconds之间随机偏移。滑动刷新过期时间。
    /// </summary>
    /// <typeparam name="TItem">值的类型</typeparam>
    /// <param name="cacheKey">缓存键</param>
    /// <param name="valueFactory">值的创建方法</param>
    /// <param name="expireSeconds">缓存过期时间（以秒为单位），默认为60秒</param>
    /// <returns>缓存值</returns>
    public static async Task<TItem?> GetOrCreateAsync<TItem>(this IDistributedCache distributedCache, string cacheKey, Func<DistributedCacheEntryOptions, Task<TItem>> valueFactory, int expireSeconds = 60)
    {
        ValidateValueType<TItem>();
        string? jsonResult = distributedCache.GetString(cacheKey);
        if (jsonResult == null)
        {
            var options = InitDistributedCacheEntryOptions(expireSeconds);
            TItem? result = await valueFactory(options).ConfigureAwait(false);
            jsonResult = JsonSerializer.Serialize(result, typeof(TItem));
            await distributedCache.SetStringAsync(cacheKey, jsonResult, options);
            return result;
        }
        else
        {
            distributedCache.Refresh(cacheKey);
            return JsonSerializer.Deserialize<TItem>(jsonResult);
        }
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
    /// 初始化分布式缓存配置项，设置过期时间为expireSeconds至两倍expireSeconds之间随机偏移。
    /// </summary>
    /// <param name="expireSeconds">过期时间（秒）。</param>
    private static DistributedCacheEntryOptions InitDistributedCacheEntryOptions(int expireSeconds)
    {
        double sec = Random.Shared.NextDouble(expireSeconds, expireSeconds * 2);
        TimeSpan expire = TimeSpan.FromSeconds(sec);
        DistributedCacheEntryOptions options = new()
        {
            AbsoluteExpirationRelativeToNow = expire
        };
        return options;
    }
}

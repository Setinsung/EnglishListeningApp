using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Commons.ASPNETCore;

/// <summary>
/// 分布式缓存帮助类，验证不允许是IEnumerable和IQueryable，避开延迟加载。同时设置过期时间为expireSeconds至两倍expireSeconds之间随机偏移
/// </summary>
public class DistributedCacheHelper : IDistributedCacheHelper
{
    private readonly IDistributedCache _distributedCache;

    public DistributedCacheHelper(IDistributedCache distributedCache)
    {
        this._distributedCache = distributedCache;
    }
    public TItem? GetOrCreate<TItem>(string cacheKey, Func<DistributedCacheEntryOptions, TItem> valueFactory, int expireSeconds = 60)
    {
        string? jsonResult = _distributedCache.GetString(cacheKey);
        if (jsonResult == null)
        {
            ValidateValueType<TItem>();
            var options = InitDistributedCacheEntryOptions(expireSeconds);
            TItem? result = valueFactory(options); // 数据源存储的null值会被json序列化为字符串"null"
            jsonResult = JsonSerializer.Serialize(result, typeof(TItem));
            _distributedCache.SetString(cacheKey, jsonResult, options);
            return result;
        }
        else
        {
            _distributedCache.Refresh(cacheKey); // 刷新滑动过期时间
            return JsonSerializer.Deserialize<TItem>(jsonResult);
        }
    }

    public async Task<TItem?> GetOrCreateAsync<TItem>(string cacheKey, Func<DistributedCacheEntryOptions, Task<TItem>> valueFactory, int expireSeconds = 60)
    {
        ValidateValueType<TItem>();
        string? jsonResult = _distributedCache.GetString(cacheKey);
        if (jsonResult == null)
        {
            var options = InitDistributedCacheEntryOptions(expireSeconds);
            TItem? result = await valueFactory(options).ConfigureAwait(false);
            jsonResult = JsonSerializer.Serialize(result, typeof(TItem));
            await _distributedCache.SetStringAsync(cacheKey, jsonResult, options);
            return result;
        }
        else
        {
            _distributedCache.Refresh(cacheKey);
            return JsonSerializer.Deserialize<TItem>(jsonResult);
        }
    }

    public void Remove(string cacheKey)
    {
        _distributedCache.Remove(cacheKey);
    }

    public Task RemoveAsync(string cacheKey)
    {
        return _distributedCache.RemoveAsync(cacheKey);
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

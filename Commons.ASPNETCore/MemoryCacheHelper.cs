﻿using Microsoft.Extensions.Caching.Memory;

namespace Commons.ASPNETCore;

/// <summary>
/// 内存缓存帮助类，验证不允许是IEnumerable和IQueryable，避开延迟加载。同时设置过期时间为expireSeconds至两倍expireSeconds之间随机偏移
/// </summary>
public class MemoryCacheHelper : IMemoryCacheHelper
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheHelper(IMemoryCache memoryCache)
    {
        this._memoryCache = memoryCache;
    }
    public TItem? GetOrCreate<TItem>(string cacheKey, Func<ICacheEntry, TItem> valueFactory, int expireSeconds = 60)
    {
        ValidateValueType<TItem>();
        if (!_memoryCache.TryGetValue(cacheKey, out TItem? result))
        {
            using ICacheEntry entry = _memoryCache.CreateEntry(cacheKey);
            InitCacheEntry(entry, expireSeconds);
            result = valueFactory(entry);
            entry.Value = result;
        }
        return result;
    }

    public async Task<TItem?> GetOrCreateAsync<TItem>(string cacheKey, Func<ICacheEntry, Task<TItem>> valueFactory, int expireSeconds = 60)
    {
        ValidateValueType<TItem>();
        if (!_memoryCache.TryGetValue(cacheKey, out TItem? result))
        {
            using ICacheEntry entry = _memoryCache.CreateEntry(cacheKey);
            InitCacheEntry(entry, expireSeconds);
            result = await valueFactory(entry).ConfigureAwait(false);
            entry.Value = result;
        }
        return result;
    }

    public void Remove(string cacheKey)
    {
        _memoryCache.Remove(cacheKey);
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

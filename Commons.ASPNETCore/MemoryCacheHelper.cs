using Microsoft.Extensions.Caching.Memory;
using System.Collections;

namespace Commons.ASPNETCore;

public class MemoryCacheHelper : IMemoryCacheHelper
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheHelper(IMemoryCache memoryCache)
    {
        this._memoryCache = memoryCache;
    }
    public T? GetOrCreate<T>(string cacheKey, Func<ICacheEntry, T?> valueFactory, int expireSeconds = 60)
    {
        ValidateValueType<T>();
        if (!_memoryCache.TryGetValue(cacheKey, out T? result))
        {
            using ICacheEntry entry = _memoryCache.CreateEntry(cacheKey);
            InitCacheEntry(entry, expireSeconds);
            result = valueFactory(entry)!;
            entry.Value = result;
        }
        return result;
    }

    public async Task<T?> GetOrCreateAsync<T>(string cacheKey, Func<ICacheEntry, Task<T?>> valueFactory, int expireSeconds = 60)
    {
        ValidateValueType<T>();
        if (!_memoryCache.TryGetValue(cacheKey, out T? result))
        {
            using ICacheEntry entry = _memoryCache.CreateEntry(cacheKey);
            InitCacheEntry(entry, expireSeconds);
            result = (await valueFactory(entry))!;
            entry.Value = result;
        }
        return result;
    }

    public void Remove(string cacheKey)
    {
        _memoryCache.Remove(cacheKey);
    }

    private static void ValidateValueType<T>()
    {
        Type typeResult = typeof(T);
        if (typeResult.IsGenericType)
        {
            typeResult = typeResult.GetGenericTypeDefinition(); // 获取泛型定义类型
        }
        if (typeResult == typeof(IEnumerable<>) ||
            typeResult == typeof(IEnumerable) ||
            typeResult == typeof(IAsyncEnumerable<T>) ||
            typeResult == typeof(IQueryable<T>) ||
            typeResult == typeof(IQueryable))
        {
            throw new InvalidOperationException($"T of {typeResult} is not allowed, please use List<T> or T[] instead.");
        }
    }

    private static void InitCacheEntry(ICacheEntry entry, int expireSeconds)
    {
        double sec = Random.Shared.NextDouble(expireSeconds, expireSeconds * 2);
        TimeSpan expire = TimeSpan.FromSeconds(sec);
        entry.AbsoluteExpirationRelativeToNow = expire;
    }
}

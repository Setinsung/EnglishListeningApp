﻿using Microsoft.Extensions.Caching.Memory;

namespace Commons.ASPNETCore;

/// <summary>
/// 内存缓存帮助类接口
/// </summary>
public interface IMemoryCacheHelper
{
    /// <summary>
    /// 获取或创建指定缓存键的值
    /// </summary>
    /// <typeparam name="T">值的类型</typeparam>
    /// <param name="cacheKey">缓存键</param>
    /// <param name="valueFactory">值的创建方法</param>
    /// <param name="expireSeconds">缓存过期时间（以秒为单位），默认为60秒</param>
    /// <returns>缓存值</returns>
    T? GetOrCreate<T>(string cacheKey, Func<ICacheEntry, T?> valueFactory, int expireSeconds = 60);

    /// <summary>
    /// 异步获取或创建指定缓存键的值
    /// </summary>
    /// <typeparam name="T">值的类型</typeparam>
    /// <param name="cacheKey">缓存键</param>
    /// <param name="valueFactory">值的创建方法</param>
    /// <param name="expireSeconds">缓存过期时间（以秒为单位），默认为60秒</param>
    /// <returns>缓存值</returns>
    Task<T?> GetOrCreateAsync<T>(string cacheKey, Func<ICacheEntry, Task<T?>> valueFactory, int expireSeconds = 60);


    /// <summary>
    /// 移除指定缓存键的值
    /// </summary>
    /// <param name="cacheKey">缓存键</param>
    void Remove(string cacheKey);
}
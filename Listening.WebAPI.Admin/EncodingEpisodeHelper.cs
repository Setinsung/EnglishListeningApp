using Commons.Helpers;
using StackExchange.Redis;

namespace Listening.WebAPI.Admin;

public class EncodingEpisodeHelper
{
    private readonly IConnectionMultiplexer _redisConn;

    public EncodingEpisodeHelper(IConnectionMultiplexer redisConn)
    {
        this._redisConn = redisConn;
    }

    private static string GetKeyForEncodingEpisodeIdsOfAlbum(Guid albumId)
    {
        return $"Listening.EncodingEpisodeIdsOfAlbum.{albumId}";
    }

    private static string GetStatusKeyForEpisode(Guid episodeId)
    {
        return $"Listening.EncodingEpisode.{episodeId}";
    }

    /// <summary>
    /// Redis中添加一个音频转码任务状态信息。同时记录专辑ID键的音频转码任务ID集合。
    /// </summary>
    /// <param name="episodeId">剧集 ID。</param>
    /// <param name="episodeInfo">音频转码任务状态信息。</param>
    /// <returns>表示异步操作的任务。</returns>
    public async Task AddEncodingEpisodeAsync(Guid episodeId, EncodingEpisodeInfo episodeInfo)
    {
        string redisKeyForEpisode = GetStatusKeyForEpisode(episodeId);
        var db = _redisConn.GetDatabase();
        // KV保存转码任务详细信息，供转码完成后插入数据库
        await db.StringSetAsync(redisKeyForEpisode, episodeInfo.ToJsonString());
        string keyForEncodingEpisodeIdsOfAlbum = GetKeyForEncodingEpisodeIdsOfAlbum(episodeInfo.AlbumId);
        // 同时Set保存此album下的episode的Id
        await db.SetAddAsync(keyForEncodingEpisodeIdsOfAlbum, episodeId.ToString());
    }

    /// <summary>
    /// 从Redis中获取指定专辑下的所有音频转码任务ID。
    /// </summary>
    /// <param name="albumId">专辑 ID。</param>
    /// <returns>表示异步操作的任务，包含音频转码任务ID列表。</returns>
    public async Task<IEnumerable<Guid>> GetEncodingEpisodeIdsAsync(Guid albumId)
    {
        string keyForEncodingEpisodeIdsOfAlbum = GetKeyForEncodingEpisodeIdsOfAlbum(albumId);
        var db = _redisConn.GetDatabase();
        var value = await db.SetMembersAsync(keyForEncodingEpisodeIdsOfAlbum);
        return value.Select(v => Guid.Parse(v));
    }

    /// <summary>
    /// 从Redis中删除一个音频转码任务状态信息。同时在专辑ID键的音频转码任务ID集合中删除。
    /// </summary>
    /// <param name="episodeId">音频ID。</param>
    /// <param name="albumId">专辑ID。</param>
    /// <returns>表示异步操作的任务，表示删除操作是否成功。</returns>
    public async Task<bool> RemoveEncodingEpisodeAsync(Guid episodeId, Guid albumId)
    {
        string redisKeyForEpisode = GetStatusKeyForEpisode(episodeId);
        var db = _redisConn.GetDatabase();
        await db.KeyDeleteAsync(redisKeyForEpisode);
        string keyForEncodingEpisodeIdsOfAlbum = GetKeyForEncodingEpisodeIdsOfAlbum(albumId);
        await db.SetRemoveAsync(keyForEncodingEpisodeIdsOfAlbum,episodeId.ToString());
        return true;
    }


    /// <summary>
    /// 更新Redis中的音频转码任务状态信息。
    /// </summary>
    /// <param name="episodeId">音频ID。</param>
    /// <param name="status">新的编码状态。</param>
    /// <returns>表示异步操作的任务，表示更新操作是否成功。</returns>
    public async Task<bool> UpdateEpisodeStatusAsync(Guid episodeId, string status)
    {
        string redisKeyForEpisode = GetStatusKeyForEpisode(episodeId);
        var db = _redisConn.GetDatabase();
        string? json = await db.StringGetAsync(redisKeyForEpisode);
        if (json == null) return false;
        if(!json.TryParseJson<EncodingEpisodeInfo>(out var episode)) return false;
        episode = episode! with { Status = status };
        await db.StringSetAsync(redisKeyForEpisode, episode.ToJsonString());
        return true;
    }


    /// <summary>
    /// 从Redis中获取音频转码任务状态信息。
    /// </summary>
    /// <param name="episodeId">音频ID。</param>
    /// <returns>表示异步操作的任务，包含转码音频的信息。</returns>
    public async Task<EncodingEpisodeInfo?> GetEncodingEpisodeAsync(Guid episodeId)
    {
        string redisKeyForEpisode = GetStatusKeyForEpisode(episodeId);
        var db = _redisConn.GetDatabase();
        string? json = await db.StringGetAsync(redisKeyForEpisode);
        if (json == null) return null;
        _ = json.TryParseJson<EncodingEpisodeInfo>(out var episode);
        return episode;
    }
}

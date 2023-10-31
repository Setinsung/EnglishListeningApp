using Commons.Domain.Models;
using Commons.Helpers;
using Listening.Domain.Entities;

namespace Listening.Domain;

public class ListeningDomainService
{
    private readonly IListeningRepository _listeningRepository;

    public ListeningDomainService(IListeningRepository listeningRepository)
    {
        this._listeningRepository = listeningRepository;
    }

    /// <summary>
    /// 添加一个专辑到指定分类下
    /// </summary>
    /// <param name="categoryId">分类ID</param>
    /// <param name="name">专辑名称</param>
    /// <returns>新添加的专辑</returns>
    public async Task<Album?> AddAlbumAsync(Guid categoryId, MultilingualString name)
    {
        if (await _listeningRepository.GetCategoryByIdAsync(categoryId) == null) return null;
        int maxSeq = await _listeningRepository.GetMaxSeqOfAlbumsAsync(categoryId);
        var album = Album.Create(Guid.NewGuid(), maxSeq + 1, name, categoryId);
        await _listeningRepository.AddAlbumAsync(album);
        return album;
    }

    /// <summary>
    /// 对指定分类下的专辑进行排序
    /// </summary>
    /// <param name="categoryId">分类ID</param>
    /// <param name="sortedAlbumIds">排序后的专辑ID列表</param>
    public async Task SortAlbumsAsync(Guid categoryId, IEnumerable<Guid> sortedAlbumIds)
    {
        IEnumerable<Album> albums = await _listeningRepository.GetAlbumsByCategoryIdAsync(categoryId);
        var idsInDB = albums.Select(a => a.Id);
        if (!idsInDB.SequenceEqualIgnoreOrder(sortedAlbumIds)) throw new Exception($"提交的待排序Id中必须是categoryId={categoryId}分类下所有的Id");
        int seqNum = 1;
        foreach (var albumId in sortedAlbumIds)
        {
            var album = await _listeningRepository.GetAlbumByIdAsync(albumId)
                ?? throw new Exception($"albumId={albumId}不存在");
            album.ChangeSequenceNumber(seqNum++);
        }
    }

    /// <summary>
    /// 添加一个分类
    /// </summary>
    /// <param name="name">分类名称</param>
    /// <param name="coverUrl">分类封面URL</param>
    /// <returns>新添加的分类</returns>
    public async Task<Category> AddCategoryAsync(MultilingualString name, Uri coverUrl)
    {
        int maxSeq = await _listeningRepository.GetMaxSeqOfCategoriesAsync();
        var category = Category.Create(Guid.NewGuid(), maxSeq + 1, name, coverUrl);
        await _listeningRepository.AddCategoryAsync(category);
        return category;
    }

    /// <summary>
    /// 对所有分类进行排序
    /// </summary>
    /// <param name="sortedCategoryIds">排序后的分类ID列表</param>
    public async Task SortCategoriesAsync(IEnumerable<Guid> sortedCategoryIds)
    {
        IEnumerable<Category> categories = await _listeningRepository.GetCategoriesAsync();
        var idsInDB = categories.Select(a => a.Id);
        if (!idsInDB.SequenceEqualIgnoreOrder(sortedCategoryIds)) throw new Exception($"提交的待排序Id中必须是所有的分类Id");
        int seqNum = 1;
        foreach (var categoryId in sortedCategoryIds)
        {
            var category = await _listeningRepository.GetCategoryByIdAsync(categoryId)
                ?? throw new Exception($"categoryId={categoryId}不存在");
            category.ChangeSequenceNumber(seqNum++);
        }
    }

    /// <summary>
    /// 添加一个音频到指定专辑下
    /// </summary>
    /// <param name="name">音频名称</param>
    /// <param name="albumId">专辑ID</param>
    /// <param name="audioUrl">音频URL</param>
    /// <param name="durationInSecond">音频时长（秒）</param>
    /// <param name="subtitleType">字幕类型</param>
    /// <param name="subtitle">字幕</param>
    /// <returns>新添加的音频</returns>
    public async Task<Episode?> AddEpisodeAsync(MultilingualString name, Guid albumId, Uri audioUrl, double durationInSecond, string subtitleType, string subtitle)
    {
        if (await _listeningRepository.GetAlbumByIdAsync(albumId) == null) return null;
        int maxSeq = await _listeningRepository.GetMaxSeqOfEpisodesAsync(albumId);
        var builder = new Episode.Builder();
        builder.Id(Guid.NewGuid()).SequenceNumber(maxSeq + 1).Name(name)
            .AlbumId(albumId).AudioUrl(audioUrl).DurationInSecond(durationInSecond)
            .SubtitleType(subtitleType).Subtitle(subtitle);
        var episode = builder.Build();
        await _listeningRepository.AddEpisodeAsync(episode);
        return episode;
    }

    /// <summary>
    /// 对指定专辑下的音频进行排序
    /// </summary>
    /// <param name="albumId">专辑ID</param>
    /// <param name="sortedEpisodeIds">排序后的音频ID列表</param>
    public async Task SortEpisodesAsync(Guid albumId, IEnumerable<Guid> sortedEpisodeIds)
    {
        IEnumerable<Episode> episodes = await _listeningRepository.GetEpisodesByAlbumIdAsync(albumId);
        var idsInDB = episodes.Select(e => e.Id);
        if (!sortedEpisodeIds.SequenceEqualIgnoreOrder(idsInDB)) throw new Exception($"提交的待排序Id中必须是albumId={albumId}专辑下所有的Id");
        int seqNum = 1;
        foreach (Guid episodeId in sortedEpisodeIds)
        {
            var episode = await _listeningRepository.GetEpisodeByIdAsync(episodeId) 
                ?? throw new Exception($"episodeId={episodeId}不存在");
            episode.ChangeSequenceNumber(seqNum++);
        }
    }
}

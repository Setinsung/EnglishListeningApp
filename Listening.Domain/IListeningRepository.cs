using Listening.Domain.Entities;

namespace Listening.Domain;

/// <summary>
/// 定义了与听力相关的数据访问方法的接口。
/// </summary>
public interface IListeningRepository
{
    /// <summary>
    /// 根据分类ID异步获取分类信息。
    /// </summary>
    /// <param name="categoryId">分类ID。</param>
    /// <returns>与给定ID相匹配的分类信息，如果找不到则返回null。</returns>
    public Task<Category?> GetCategoryByIdAsync(Guid categoryId);

    /// <summary>
    /// 异步获取所有分类信息。
    /// </summary>
    /// <returns>包含所有分类信息的集合。</returns>
    public Task<Category[]> GetCategoriesAsync();

    /// <summary>
    /// 异步获取分类信息中最大的序号。
    /// </summary>
    /// <returns>最大的序号。</returns>
    public Task<int> GetMaxSeqOfCategoriesAsync();

    /// <summary>
    /// 添加类别。
    /// </summary>
    /// <param name="category">类别实体。</param>
    /// <returns></returns>
    public Task AddCategoryAsync(Category category);


    /// <summary>
    /// 根据专辑ID异步获取专辑信息。
    /// </summary>
    /// <param name="albumId">专辑ID。</param>
    /// <returns>与给定ID相匹配的专辑信息，如果找不到则返回null。</returns>
    public Task<Album?> GetAlbumByIdAsync(Guid albumId);

    /// <summary>
    /// 根据分类ID异步获取该分类下所有专辑信息中的最大序号。
    /// </summary>
    /// <param name="categoryId">分类ID。</param>
    /// <returns>最大的序号。</returns>
    public Task<int> GetMaxSeqOfAlbumsAsync(Guid categoryId);

    /// <summary>
    /// 根据分类ID异步获取该分类下的所有专辑信息。
    /// </summary>
    /// <param name="categoryId">分类ID。</param>
    /// <returns>包含分类下所有专辑信息的集合。</returns>
    public Task<Album[]> GetAlbumsByCategoryIdAsync(Guid categoryId);
    
    /// <summary>
    /// 添加专辑。
    /// </summary>
    /// <param name="album">专辑实体。</param>
    /// <returns></returns>
    public Task AddAlbumAsync(Album album);


    /// <summary>
    /// 根据音频ID异步获取音频信息。
    /// </summary>
    /// <param name="episodeId">音频ID。</param>
    /// <returns>与给定ID相匹配的音频信息，如果找不到则返回null。</returns>
    public Task<Episode?> GetEpisodeByIdAsync(Guid episodeId);


    /// <summary>
    /// 根据专辑ID异步获取该专辑下所有音频信息中的最大序号。
    /// </summary>
    /// <param name="albumId">专辑ID。</param>
    /// <returns>最大的序号。</returns>
    public Task<int> GetMaxSeqOfEpisodesAsync(Guid albumId);


    /// <summary>
    /// 根据专辑ID异步获取该专辑下的所有音频信息。
    /// </summary>
    /// <param name="albumId">专辑ID。</param>
    /// <returns>包含专辑下所有音频信息的集合。</returns>
    public Task<Episode[]> GetEpisodesByAlbumIdAsync(Guid albumId);

    /// <summary>
    /// 添加音频。
    /// </summary>
    /// <param name="episode">音频实体。</param>
    /// <returns></returns>
    public Task AddEpisodeAsync(Episode episode);

}

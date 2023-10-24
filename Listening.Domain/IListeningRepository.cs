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
    public Task<IEnumerable<Category>> GetCategoriesAsync();

    /// <summary>
    /// 异步获取分类信息中最大的序号。
    /// </summary>
    /// <returns>最大的序号。</returns>
    public Task<int> GetMaxSeqOfCategoriesAsync();


    /// <summary>
    /// 根据专辑ID异步获取专辑信息。
    /// </summary>
    /// <param name="albumId">专辑ID。</param>
    /// <returns>与给定ID相匹配的专辑信息，如果找不到则返回null。</returns>
    public Task<Album?> GetAlbumByIdAsync(Guid albumId);

    /// <summary>
    /// 根据分类ID异步获取该分类下的所有专辑信息。
    /// </summary>
    /// <param name="categoryId">分类ID。</param>
    /// <returns>包含分类下所有专辑信息的集合。</returns>
    public Task<IEnumerable<Album>> GetAlbumsByCategoryIdAsync(Guid categoryId);

    /// <summary>
    /// 根据单集ID异步获取单集信息。
    /// </summary>
    /// <param name="episodeId">单集ID。</param>
    /// <returns>与给定ID相匹配的单集信息，如果找不到则返回null。</returns>
    public Task<Episode?> GetEpisodeByIdAsync(Guid episodeId);


    /// <summary>
    /// 根据专辑ID异步获取该专辑下所有单集信息中的最大序号。
    /// </summary>
    /// <param name="albumId">专辑ID。</param>
    /// <returns>最大的序号。</returns>
    public Task<int> GetMaxSeqOfEpisodesAsync(Guid albumId);


    /// <summary>
    /// 根据专辑ID异步获取该专辑下的所有单集信息。
    /// </summary>
    /// <param name="albumId">专辑ID。</param>
    /// <returns>包含专辑下所有单集信息的集合。</returns>
    public Task<IEnumerable<Episode>> GetEpisodesByAlbumIdAsync(Guid albumId);

}

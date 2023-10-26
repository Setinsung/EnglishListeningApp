using Commons.Infrastructure;
using Listening.Domain;
using Listening.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Listening.Infrastructure;

public class ListeningRepository : IListeningRepository
{
    private readonly ListeningDbContext _listeningDbContext;

    public ListeningRepository(ListeningDbContext listeningDbContext)
    {
        this._listeningDbContext = listeningDbContext;
    }
    public async Task AddAlbumAsync(Album album)
    {
        await _listeningDbContext.AddAsync(album);
    }

    public async Task AddCategoryAsync(Category category)
    {
        await _listeningDbContext.AddAsync(category);
    }

    public async Task AddEpisodeAsync(Episode episode)
    {
        await _listeningDbContext.AddAsync(episode);
    }

    public async Task<Album?> GetAlbumByIdAsync(Guid albumId)
    {
        return await _listeningDbContext.FindAsync<Album>(albumId);
    }

    public Task<Album[]> GetAlbumsByCategoryIdAsync(Guid categoryId)
    {
        return _listeningDbContext.Albums
            .OrderBy(e => e.SequenceNumber)
            .Where(e => e.CategoryId == categoryId).ToArrayAsync();
    }

    public Task<Category[]> GetCategoriesAsync()
    {
        return _listeningDbContext.Categories
            .OrderBy(e => e.SequenceNumber).ToArrayAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(Guid categoryId)
    {
        return await _listeningDbContext.FindAsync<Category>(categoryId);
    }

    public async Task<Episode?> GetEpisodeByIdAsync(Guid episodeId)
    {
        return await _listeningDbContext.Episodes.SingleOrDefaultAsync(e => e.Id == episodeId);
    }

    public Task<Episode[]> GetEpisodesByAlbumIdAsync(Guid albumId)
    {
        return _listeningDbContext.Episodes
            .OrderBy(e=>e.SequenceNumber)
            .Where(e => e.AlbumId ==  albumId).ToArrayAsync();
    }

    public async Task<int> GetMaxSeqOfAlbumsAsync(Guid categoryId)
    {
        // e => (int?)e.SequenceNumber在0条数据时不报错
        int? maxSeq = await _listeningDbContext.Query<Album>()
            .Where(e=>e.CategoryId == categoryId)
            .MaxAsync(e => (int?)e.SequenceNumber);
        return maxSeq ?? 0;
    }

    public async Task<int> GetMaxSeqOfCategoriesAsync()
    {
        int? maxSeq = await _listeningDbContext.Query<Category>()
            .MaxAsync(e => (int?)e.SequenceNumber);
        return maxSeq ?? 0;
    }

    public async Task<int> GetMaxSeqOfEpisodesAsync(Guid albumId)
    {
        int? maxSeq = await _listeningDbContext.Query<Episode>()
            .Where(e => e.AlbumId == albumId)
            .MaxAsync(e => (int?)e.SequenceNumber);
        return maxSeq ?? 0;
    }
}

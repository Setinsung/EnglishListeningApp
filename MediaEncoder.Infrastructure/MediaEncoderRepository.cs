using MediaEncoder.Domain;
using MediaEncoder.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaEncoder.Infrastructure;

public class MediaEncoderRepository : IMediaEncoderRepository
{
    private readonly MediaEncoderDbContext _mediaEncoderDbContext;

    public MediaEncoderRepository(MediaEncoderDbContext mediaEncoderDbContext)
    {
        this._mediaEncoderDbContext = mediaEncoderDbContext;
    }
    public Task<EncodingItem[]> FindAsync(ItemStatus status)
    {
        return _mediaEncoderDbContext.EncodingItems.Where(e => e.Status == ItemStatus.Ready).ToArrayAsync();
    }

    public Task<EncodingItem?> FindCompletedOneAsync(string fileHash, long fileSize)
    {
        return _mediaEncoderDbContext.EncodingItems.FirstOrDefaultAsync(e => e.FileSHA256Hash == fileHash && e.FileSizeInBytes == fileSize && e.Status == ItemStatus.Completed);
    }
}

using FileService.Domain;
using FileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileService.Infrastructure;

public class FileServiceRepository : IFileServiceRepository
{
    private readonly FileUploadDbContext _fileUploadDbContext;

    public FileServiceRepository(FileUploadDbContext fileUploadDbContext)
    {
        this._fileUploadDbContext = fileUploadDbContext;
    }

    public Task<UploadedItem?> FindFileAsync(long fileSize, string sha256Hash)
    {
        return _fileUploadDbContext.UploadedItems
            .FirstOrDefaultAsync(e => e.FileSizeBytes == fileSize && e.FileSHA256Hash == sha256Hash);
    }
    public async Task<UploadedItem> FindFileOrCreateAsync(long fileSize, string fileHash, Func<Task<UploadedItem>> valueFactory)
    {
        UploadedItem? uploadedItem = await FindFileAsync(fileSize, fileHash);
        if (uploadedItem == null)
        {
            uploadedItem = await valueFactory();
            await _fileUploadDbContext.UploadedItems.AddAsync(uploadedItem);
        }
        return uploadedItem;
    }
}

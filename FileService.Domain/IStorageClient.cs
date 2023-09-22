namespace FileService.Domain
{
    public interface IStorageClient
    {
        StorageType StorageType { get; set; }

        Task<Uri> SaveFileAsync(string key, Stream content, CancellationToken cancellationToken = default);
    }
}

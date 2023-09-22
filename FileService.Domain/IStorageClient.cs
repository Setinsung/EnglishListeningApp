namespace FileService.Domain;

public interface IStorageClient
{
    StorageType StorageType { get; }

    Task<Uri> SaveFileAsync(string key, Stream content, CancellationToken cancellationToken = default);
}

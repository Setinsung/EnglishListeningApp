using FileService.Domain;
using Microsoft.Extensions.Options;

namespace FileService.Infrastructure.Services;

public class SMBStorageClient : IStorageClient
{
    private readonly IOptionsSnapshot<SMBStorageOptions> _options;

    public StorageType StorageType => StorageType.Backup;

    public SMBStorageClient(IOptionsSnapshot<SMBStorageOptions> options)
    {
        this._options = options;
    }
    public async Task<Uri> SaveFileAsync(string key, Stream content, CancellationToken cancellationToken = default)
    {
        if (key.StartsWith("/")) throw new ArgumentException("key should not start with /", nameof(key));
        string workDir = _options.Value.WorkingDir;
        string fullPath = Path.Combine(workDir, key);
        string? fullDir = Path.GetDirectoryName(fullPath);
        Directory.CreateDirectory(fullDir);
        if (File.Exists(fullPath)) File.Delete(fullPath); // 已存在删除
        using Stream outStream = File.OpenWrite(fullPath);
        await content.CopyToAsync(outStream, cancellationToken);
        return new Uri(fullPath);
    }
}

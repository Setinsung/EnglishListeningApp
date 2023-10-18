using FileService.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace FileService.Infrastructure.Services;

/// <summary>
/// 模拟使用，实际保存在wwwroot下
/// </summary>
public class MockCloudStorageClient : IStorageClient
{
    private readonly IWebHostEnvironment _hostEnv;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public StorageType StorageType => StorageType.Public;

    public MockCloudStorageClient(IWebHostEnvironment hostEnv, IHttpContextAccessor httpContextAccessor)
    {
        this._hostEnv = hostEnv;
        this._httpContextAccessor = httpContextAccessor;
    }
    public async Task<Uri> SaveFileAsync(string key, Stream content, CancellationToken cancellationToken = default)
    {
        if(key.StartsWith("/")) throw new ArgumentException("key should not start with /", nameof(key));
        string workingDir = Path.Combine(_hostEnv.ContentRootPath, "wwwroot");
        string fullPath = Path.Combine(workingDir, key);
        string? fullDir = Path.GetDirectoryName(fullPath);
        Directory.CreateDirectory(fullDir);
        if(File.Exists(fullPath)) File.Delete(fullPath); // 已存在则删除
        using Stream outStream = File.OpenWrite(fullPath);
        await content.CopyToAsync(outStream,cancellationToken);
        var req = _httpContextAccessor.HttpContext.Request;
        string url = req.Scheme + "://" + req.Host + "/FileService/" + key;
        return new Uri(url);
    }
}

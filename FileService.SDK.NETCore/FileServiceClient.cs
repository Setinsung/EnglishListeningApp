using Commons.Helpers;
using Commons.JWT;
using System.Security.Claims;

namespace FileService.SDK.NETCore;
/// <summary>
/// 请求到FileService的文件上传API供其它微服务调用
/// </summary>
public class FileServiceClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Uri _serverRoot;
    private readonly JWTOptions _jwtOptions;
    private readonly ITokenService _tokenService;

    public FileServiceClient(IHttpClientFactory httpClientFactory, Uri serverRoot, JWTOptions jwtOptions, ITokenService tokenService)
    {
        this._httpClientFactory = httpClientFactory;
        this._serverRoot = serverRoot;
        this._jwtOptions = jwtOptions;
        this._tokenService = tokenService;
    }

    /// <summary>
    /// 请求FileService检查文件是否已存在
    /// </summary>
    /// <param name="fileSize">文件大小</param>
    /// <param name="sha256Hash">SHA256哈希值</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>文件存在响应</returns>
    public async Task<FileExistsResponse> FileExistsAsync(long fileSize, string sha256Hash, CancellationToken cancellationToken = default)
    {
        string relativeUrl = FormattableStringHelper.BuildUrl($"api/Uploader/FileExists?fileSize={fileSize}&sha256Hash={sha256Hash}");
        Uri requestUri = new Uri(_serverRoot, relativeUrl);
        var httpClient = _httpClientFactory.CreateClient();
        FileExistsResponse? fileExistsResponse = await httpClient.GetJsonAsync<FileExistsResponse>(requestUri, cancellationToken);
        return fileExistsResponse ?? new FileExistsResponse(false, null);
    }

    /// <summary>
    /// 请求FileService上传文件
    /// </summary>
    /// <param name="file">文件信息</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>上传后的文件URL</returns>
    public async Task<Uri> UploadAsync(FileInfo file, CancellationToken cancellationToken = default)
    {
        string token = BuildToken();
        using MultipartFormDataContent content = new MultipartFormDataContent(); // multipart/form-data类型HTTP请求体，用于上传文件
        using var fileContent = new StreamContent(file.OpenRead());
        content.Add(fileContent, "file", file.Name);
        var httpClient = _httpClientFactory.CreateClient();
        Uri requestUrl = new Uri(_serverRoot + "/Uploader/Upload");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token); // 设置httpClient的默认请求头中的授权信息，使用Authorization头部并传递令牌
        var respMsg = await httpClient.PostAsync(requestUrl, content, cancellationToken);
        string respString = await respMsg.Content.ReadAsStringAsync(cancellationToken);
        if (!respMsg.IsSuccessStatusCode)
            throw new HttpRequestException($"上传失败，状态码：{respMsg.StatusCode}，响应报文：{respString}");
        else
            return respString.ParseJson<Uri>()!;
    }

    private string BuildToken()
    {
        // JWT密钥信息就存储在服务器端，可以简单地读到配置
        Claim claim = new Claim(ClaimTypes.Role, "Admin");
        return _tokenService.BuildToken(new Claim[] { claim }, _jwtOptions);
    }
}

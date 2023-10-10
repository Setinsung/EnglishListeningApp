namespace FileService.WebAPI.ViewModels;

/// <summary>
/// 表示文件是否存在的响应对象
/// </summary>
/// <param name="IsExists">表示文件是否存在的响应对象</param>
/// <param name="url">如果文件存在，则代表这个文件的路径</param>
public record FileExistsResponse(bool IsExists, Uri? Url);


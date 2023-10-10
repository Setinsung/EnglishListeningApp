using Commons.ASPNETCore;
using FileService.Domain;
using FileService.Infrastructure;
using FileService.WebAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileService.WebAPI.Controllers;

[Route("[controller]/[action]")]
[ApiController]
[Authorize(Roles = "Admin")]
[UnitOfWorkFilter(typeof(FileUploadDbContext))]
public class UploaderController : ControllerBase
{
    private readonly FileDomainService _fileDomainService;
    private readonly IFileServiceRepository _fileServiceRepository;

    public UploaderController(FileDomainService fileDomainService, IFileServiceRepository fileServiceRepository)
    {
        this._fileDomainService = fileDomainService;
        this._fileServiceRepository = fileServiceRepository;
    }

    // 检查是否有和指定的大小和SHA256完全一样的文件
    [HttpGet]
    public async Task<ActionResult<FileExistsResponse>> FileExists(long fileSize, string sha256Hash)
    {
        var item = await _fileServiceRepository.FindFileAsync(fileSize, sha256Hash);
        if (item == null) return Ok(new FileExistsResponse(false, null));
        else return Ok(new FileExistsResponse(true, item.RemoteUrl));
    }

    // 上传文件
    [HttpPost]
    [RequestSizeLimit(60_000_000)]
    public async Task<ActionResult<Uri>> UploadFile([FromForm] UploadRequest uploadRequest, CancellationToken cancellationToken = default)
    {
        var file = uploadRequest.File;
        string fileName = file.FileName;
        using Stream stream = file.OpenReadStream();
        var upItem = await _fileDomainService.UploadAsync(stream, fileName, cancellationToken);
        return Ok(upItem.RemoteUrl);
    }
}

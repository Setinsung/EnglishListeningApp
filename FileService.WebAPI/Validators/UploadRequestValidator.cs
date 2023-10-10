using FileService.WebAPI.ViewModels;
using FluentValidation;

namespace FileService.WebAPI.Validators;

public class UploadRequestValidator : AbstractValidator<UploadRequest>
{
    public UploadRequestValidator()
    {
        //不用校验文件名的后缀，允许用户上传exe、php等文件，文件服务器会做好安全设置
        long maxFileSize = 50 * 1024 * 1024;
        RuleFor(e => e.File).NotNull().Must(f => f.Length > 0 && f.Length <= maxFileSize);
    }
}

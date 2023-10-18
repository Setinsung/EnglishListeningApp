namespace FileService.Infrastructure.Services;

public class SMBStorageOptions
{
    public string WorkingDir { get; set; } // 非聚合相关类set要公开，否则无法注入
}

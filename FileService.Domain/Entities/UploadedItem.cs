using Commons.Domain.Models;

namespace FileService.Domain.Entities;

public record UploadedItem : BaseEntity, ICreatedTime
{
    public DateTime CreatedTime { get; private set; }

    public long FileSizeBytes { get; private set; } // 文件大小

    public string FileName { get;private set; } // 原始文件名

    public string FileSHA256Hash { get; private set; } // 散列值

    public Uri BackupUrl { get; private set; } // 内部物理文件路径

    public Uri RemoteUrl { get; private set; } // 外部云端访问路径

    public static UploadedItem Create(Guid id, long fileSizeBytes, string FileName, string fileSHA256Hash, Uri backupUrl, Uri remoteUrl)
    {
        UploadedItem item = new()
        {
            Id = id,
            CreatedTime = DateTime.Now,
            FileSizeBytes = fileSizeBytes,
            FileName = FileName,
            FileSHA256Hash = fileSHA256Hash,
            BackupUrl = backupUrl,
            RemoteUrl = remoteUrl
        };
        return item;
    }

}

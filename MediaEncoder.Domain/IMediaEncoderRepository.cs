using MediaEncoder.Domain.Entities;

namespace MediaEncoder.Domain;

public interface IMediaEncoderRepository
{
    /// <summary>
    /// 异步查找已完成转码文件任务项
    /// </summary>
    /// <param name="fileHash">文件哈希值</param>
    /// <param name="fileSize">文件大小</param>
    /// <returns>已完成转码文件任务项，如果未找到则返回null</returns>
    Task<EncodingItem?> FindCompletedOneAsync(string fileHash, long fileSize);

    /// <summary>
    /// 异步查找符合指定状态的转码文件任务项数组
    /// </summary>
    /// <param name="status">转码文件状态</param>
    /// <returns>符合状态的转码文件任务项数组</returns>
    Task<EncodingItem[]> FindAsync(ItemStatus status);
}

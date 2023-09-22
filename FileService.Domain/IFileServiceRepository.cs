using FileService.Domain.Entities;

namespace FileService.Domain
{
    public interface IFileServiceRepository
    {
        /// <summary>
        /// 按照散列值和文件大小查找文件
        /// </summary>
        /// <param name="fileSize"></param>
        /// <param name="sha256Hash"></param>
        /// <returns></returns>
        Task<UploadedItem?> FindFileAsync(long fileSize, string sha256Hash);
    }
}

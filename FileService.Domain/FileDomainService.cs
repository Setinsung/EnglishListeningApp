using Commons.Helpers;
using FileService.Domain.Entities;

namespace FileService.Domain
{
    public class FileDomainService
    {
        private readonly IFileServiceRepository _fileServiceRepository;
        private readonly IStorageClient _backupStorage; // 备份服务器
        private readonly IStorageClient _remoteStorage; // 公开访问存储服务器
        public FileDomainService(IFileServiceRepository fileServiceRepository, IEnumerable<IStorageClient> storageClients)
        {
            this._fileServiceRepository = fileServiceRepository;
            // 内置DI不能使用变量名注入不同实例，应直接注入IEnumerable手动对应赋值
            this._backupStorage = storageClients.First(c => c.StorageType == StorageType.Backup);
            this._remoteStorage = storageClients.First(c => c.StorageType == StorageType.Public);
        }

        public async Task<UploadedItem> UploadAsync(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            string fileHash = HashHelper.ComputeSha256Hash(stream);
            long fileSize = stream.Length;
            DateTime today = DateTime.Today;
            // 文件按日期分散目录存储，再加上了hash值和文件名，不易冲突。
            string key = $"{today.Year}/{today.Month}/{today.Day}/{fileHash}/{fileName}";
            var oldItem = await _fileServiceRepository.FindFileAsync(fileSize, fileHash); // 按文件大小和hash查询文件返回，没有则创建。
            if (oldItem != null) return oldItem;

            // 上传，创建UploadedItem
            stream.Position = 0;
            Uri backUrl = await _backupStorage.SaveFileAsync(key, stream, cancellationToken);
            stream.Position = 0;
            Uri remoteUrl = await _remoteStorage.SaveFileAsync(key,stream,cancellationToken);
            stream.Position = 0;
            return UploadedItem.Create(Guid.NewGuid(), fileSize, fileName, fileHash, backUrl, remoteUrl);
            // TODO: 修改为FindOrCreateAsync，没必要把这个返回出去让应用服务层去调用dbcontext
        }
    }
}

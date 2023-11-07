namespace MediaEncoder.Domain;

public interface IMediaEncoder
{
    /// <summary>
    /// 是否能处理指定转码目标类型的文件
    /// </summary>
    /// <param name="outputFormat">转码目标文件格式</param>
    /// <returns>能处理返回true，无法处理返回false</returns>
    bool Accept(string outputFormat);

    /// <summary>
    /// 异步转码文件
    /// </summary>
    /// <param name="sourceFile">源文件</param>
    /// <param name="destFile">目标文件</param>
    /// <param name="destFormat">目标文件格式</param>
    /// <param name="args">转码参数</param>
    /// <param name="cancellationToken">取消操作的令牌</param>
    /// <returns>异步任务</returns>
    Task EncodeAsync(FileInfo sourceFile, FileInfo destFile, string destFormat, string[]? args, CancellationToken cancellationToken);
}

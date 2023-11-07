using Commons.Domain.Models;
using MediaEncoder.Domain.Events;

namespace MediaEncoder.Domain.Entities;

public record EncodingItem : BaseEntity, IAggregateRoot, ICreatedTime
{
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; private set; }

    /// <summary>
    /// 待转码文件发送源系统
    /// </summary>
    public string SourceSystem { get; private set; }

    /// <summary>
    /// 待转码文件大小(byte)
    /// </summary>
    public long? FileSizeInBytes { get; private set; }

    /// <summary>
    /// 待转码文件名称
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 待转码文件散列值
    /// </summary>
    public string? FileSHA256Hash { get; private set; }

    /// <summary>
    /// 待转码文件路径
    /// </summary>
    public Uri SourceUrl { get; private set; }

    /// <summary>
    /// 转码完成文件路径
    /// </summary>
    public Uri? OutputUrl { get; private set; }

    /// <summary>
    /// 转码目标类型，如m4a、mp4等
    /// </summary>
    public string OutputFormat { get; private set; }

    /// <summary>
    /// 转码状态
    /// </summary>
    public ItemStatus Status { get; private set; }

    /// <summary>
    /// 转码输出日志
    /// </summary>
    public string? LogText { get; private set; }

    /// <summary>
    /// 开始转码
    /// </summary>
    public void Start()
    {
        this.Status = ItemStatus.Started;
        AddDomainEvent(new EncodingItemStartedEvent(this.Id, this.SourceSystem));
    }

    /// <summary>
    /// 转码失败
    /// </summary>
    /// <param name="outputUrl">转码完成文件路径</param>
    public void Complete(Uri outputUrl)
    {
        this.Status = ItemStatus.Completed;
        this.OutputUrl = outputUrl;
        this.LogText = "转码成功";
        AddDomainEvent(new EncodingItemCompletedEvent(this.Id, this.SourceSystem, outputUrl));
    }

    /// <summary>
    /// 转码失败
    /// </summary>
    /// <param name="logText">失败日志</param>
    public void Fail(string logText)
    {
        this.Status = ItemStatus.Failed;
        this.LogText = logText;
        AddDomainEventIfAbsent(new EncodingItemFailedEvent(this.Id, this.SourceSystem, logText));
    }

    /// <summary>
    /// 转码失败
    /// </summary>
    /// <param name="ex">失败异常</param>
    public void Fail(Exception ex)
    {
        Fail($"转码处理失败：{ex}");
    }

    /// <summary>
    /// 修改文件大小和散列值
    /// </summary>
    /// <param name="fileSize"></param>
    /// <param name="hash"></param>
    public void ChangeFileMeta(long fileSize, string hash)
    {
        this.FileSizeInBytes = fileSize;
        this.FileSHA256Hash = hash;
    }

    /// <summary>
    /// 创建转码文件任务
    /// </summary>
    /// <param name="id">转码文件任务id</param>
    /// <param name="name">待转码文件名称</param>
    /// <param name="sourceUrl">待转码文件路径</param>
    /// <param name="outputFormat">转码目标类型</param>
    /// <param name="sourceSystem">待转码文件发送源系统</param>
    /// <returns>创建的转码文件任务</returns>
    public static EncodingItem Create(Guid id, string name, Uri sourceUrl, string outputFormat, string sourceSystem)
    {
        EncodingItem item = new()
        {
            Id = id,
            CreatedTime = DateTime.Now,
            Name = name,
            OutputFormat = outputFormat,
            SourceUrl = sourceUrl,
            Status = ItemStatus.Ready,
            SourceSystem = sourceSystem
        };
        item.AddDomainEvent(new EncodingItemCreatedEvent(item));
        return item;
    }

}

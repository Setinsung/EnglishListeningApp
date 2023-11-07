namespace MediaEncoder.Domain.Entities;

public enum ItemStatus
{
    /// <summary>
    /// 任务创建
    /// </summary>
    Ready,

    /// <summary>
    /// 开始处理
    /// </summary>
    Started,

    /// <summary>
    /// 成功
    /// </summary>
    Completed,

    /// <summary>
    /// 失败
    /// </summary>
    Failed,
}

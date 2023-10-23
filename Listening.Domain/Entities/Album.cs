using Commons.Domain.Models;

namespace Listening.Domain.Entities;

public record Album : AggregateRootEntity
{
    private Album() { }

    /// <summary>
    /// 用户是否可见（完善后才显示）
    /// </summary>
    public bool IsVisible { get; private set; }

    /// <summary>
    /// 标题
    /// </summary>
    public MultilingualString Name { get; private set; }

    /// <summary>
    /// 列表显示序号
    /// </summary>
    public int SequenceNumber { get; private set; }

    /// <summary>
    /// 类别
    /// </summary>
    public Guid CategoryId { get; private set; }

    public static Album Create(Guid id, int sequenceNumber, MultilingualString name, Guid categoryId)
    {
        return new Album()
        {
            Id = id,
            Name = name,
            SequenceNumber = sequenceNumber,
            CategoryId = categoryId,
            IsVisible = false // Album新建以后默认不可见，需要手动Show

        };
    }

    public Album ChangeSequenceNumber(int value)
    {
        this.SequenceNumber = value;
        return this;
    }

    public Album ChangeName(MultilingualString value)
    {
        this.Name = value;
        return this;
    }

    public Album Hide()
    {
        this.IsVisible = false;
        return this;
    }
    public Album Show()
    {
        this.IsVisible = true;
        return this;
    }
}

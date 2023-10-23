using Commons.Domain.Models;

namespace Listening.Domain.Entities;

public record Category : AggregateRootEntity
{
    private Category() { }

    /// <summary>
    /// 类别名称
    /// </summary>
    public MultilingualString Name { get; private set; }

    /// <summary>
    /// 显示序号，靠前优先
    /// </summary>
    public int SequenceNumber { get; private set; }

    /// <summary>
    /// 封面图片路径
    /// </summary>
    public Uri CoverUrl { get; private set; }

    public static Category Create(Guid id, int sequenceNumber, MultilingualString name, Uri coverUrl)
    {
        var category = new Category()
        {
            Id = id,
            Name = name,
            SequenceNumber = sequenceNumber,
            CoverUrl = coverUrl
        };
        // category.AddDomainEvent(new CategoryCreatedEventArgs { NewObj = category });
        return category;
    }
    
    public Category ChangeName(MultilingualString value)
    {
        this.Name = value;
        return this;
    }
    public Category ChangeSequenceNumber(int value)
    {
        this.SequenceNumber = value;
        return this;
    }

    public Category ChangeCoverUrl(Uri coverUrl)
    {
        // 实际应该不管这个事件是否有被用到，都应尽量publish。
        this.CoverUrl = coverUrl;
        return this;
    }
}

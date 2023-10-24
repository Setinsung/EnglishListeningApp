using Commons.Domain.Models;
using Listening.Domain.Events;

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

    /// <summary>
    /// 创建一个类别实例。
    /// </summary>
    /// <param name="id">类别的唯一标识符。</param>
    /// <param name="sequenceNumber">显示序号，靠前优先。</param>
    /// <param name="name">类别名称。</param>
    /// <param name="coverUrl">封面图片路径。</param>
    /// <returns>创建的类别实例。</returns>
    public static Category Create(Guid id, int sequenceNumber, MultilingualString name, Uri coverUrl)
    {
        var category = new Category()
        {
            Id = id,
            Name = name,
            SequenceNumber = sequenceNumber,
            CoverUrl = coverUrl
        };
        category.AddDomainEvent(new CategoryCreatedEvent(category));
        return category;
    }

    /// <summary>
    /// 修改类别名称。
    /// </summary>
    /// <param name="value">新的类别名称。</param>
    /// <returns>修改后的类别实例。</returns>
    public Category ChangeName(MultilingualString value)
    {
        this.Name = value;
        this.AddDomainEventIfAbsent(new CategoryUpdatedEvent(this));
        return this;
    }

    /// <summary>
    /// 修改显示序号。
    /// </summary>
    /// <param name="value">新的显示序号。</param>
    /// <returns>修改后的类别实例。</returns>
    public Category ChangeSequenceNumber(int value)
    {
        this.SequenceNumber = value;
        this.AddDomainEventIfAbsent(new CategoryUpdatedEvent(this));
        return this;
    }

    /// <summary>
    /// 修改封面图片路径。
    /// </summary>
    /// <param name="coverUrl">新的封面图片路径。</param>
    /// <returns>修改后的类别实例。</returns>
    public Category ChangeCoverUrl(Uri coverUrl)
    {
        this.CoverUrl = coverUrl;
        this.AddDomainEventIfAbsent(new CategoryUpdatedEvent(this));
        return this;
    }
}

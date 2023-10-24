using Commons.Domain.Models;
using Listening.Domain.Events;

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

    /// <summary>
    /// 创建专辑
    /// </summary>
    /// <param name="id">专辑ID</param>
    /// <param name="sequenceNumber">列表显示序号</param>
    /// <param name="name">标题</param>
    /// <param name="categoryId">类别ID</param>
    /// <returns>创建的专辑</returns>
    public static Album Create(Guid id, int sequenceNumber, MultilingualString name, Guid categoryId)
    {
        var album = new Album()
        {
            Id = id,
            Name = name,
            SequenceNumber = sequenceNumber,
            CategoryId = categoryId,
            IsVisible = false // Album新建以后默认不可见，需要手动Show
        };
        album.AddDomainEvent(new AlbumCreatedEvent(album));
        return album;
    }

    /// <summary>
    /// 修改列表显示序号
    /// </summary>
    /// <param name="value">新的列表显示序号</param>
    /// <returns>修改后的专辑</returns>
    public Album ChangeSequenceNumber(int value)
    {
        this.SequenceNumber = value;
        this.AddDomainEventIfAbsent(new AlbumUpdatedEvent(this));
        return this;
    }

    /// <summary>
    /// 修改标题
    /// </summary>
    /// <param name="value">新的标题</param>
    /// <returns>修改后的专辑</returns>
    public Album ChangeName(MultilingualString value)
    {
        this.Name = value;
        this.AddDomainEventIfAbsent(new AlbumUpdatedEvent(this));
        return this;
    }

    /// <summary>
    /// 隐藏专辑
    /// </summary>
    /// <returns>隐藏后的专辑</returns>
    public Album Hide()
    {
        this.IsVisible = false;
        this.AddDomainEventIfAbsent(new AlbumUpdatedEvent(this));
        return this;
    }

    /// <summary>
    /// 显示专辑
    /// </summary>
    /// <returns>显示后的专辑</returns>
    public Album Show()
    {
        this.IsVisible = true;
        this.AddDomainEventIfAbsent(new AlbumUpdatedEvent(this));
        return this;
    }
}

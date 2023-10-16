namespace Commons.Domain.Models;

public record AggregateRootEntity : BaseEntity, IAggregateRoot, ISoftDelete, ICreatedTime, IDeletedTime, IModifiedTime
{
    public bool IsDeleted { get; private set; }

    public DateTime CreatedTime { get; private set; } = DateTime.Now;   

    public DateTime? LastModifiedTime { get; private set; }

    public DateTime? DeletedTime { get; private set; }


    public virtual void SoftDelete()
    {
        this.IsDeleted = true;
        this.DeletedTime  = DateTime.Now;
    }

    public void NotifyModified()
    {
        this.LastModifiedTime = DateTime.Now;
    }
}

using MediatR;

namespace Commons.Domain.Models;

public record BaseEntity : IDomainEvents, IEntity
{
    private List<INotification> domainEvents = new(); // 存储注册的领域事件
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public void AddDomainEvent(INotification eventItem)
    {
        domainEvents.Add(eventItem);
    }

    public void AddDomainEventIfAbsent(INotification eventItem)
    {
        if(!domainEvents.Contains(eventItem))
        {
            domainEvents.Add(eventItem);
        }
    }

    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }

    public IEnumerable<INotification> GetDomainEvents()
    {
        return domainEvents;
    }
}

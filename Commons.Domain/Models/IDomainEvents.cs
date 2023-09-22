using MediatR;

namespace Commons.Domain.Models
{
    public interface IDomainEvents
    {
        IEnumerable<INotification> GetDomainEvents(); // 获取注册的领域事件
        void AddDomainEvent(INotification eventItem); // 注册领域事件
        void AddDomainEventIfAbsent(INotification eventItem); // 如果领域事件不存在才注册事件
        void ClearDomainEvents(); // 清除注册的领域事件
    }
}

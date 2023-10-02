using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Infrastructure;

public abstract class BaseDbContext : DbContext
{
    private readonly IMediator? _mediator;

    public BaseDbContext(DbContextOptions options ,IMediator? mediator) : base(options)
    {
        this._mediator = mediator;
    }
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        throw new NotImplementedException("Don not call SaveChanges, please call SaveChangesAsync instead.");
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        // 发布跟踪的所有实体中的所有事件
        if (_mediator == null) throw new ArgumentNullException("Mediator not injected.");
        var domainEntities = this.ChangeTracker.Entries<IDomainEvents>() // 获取所有EntityEntry，用于访问具体的实体类对象
            .Where(x => x.Entity.GetDomainEvents()
            .Any());
        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.GetDomainEvents())
            .ToList();
        domainEntities.ToList().ForEach(x => x.Entity.ClearDomainEvents());
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
        }
        // 已软删除实体对象取消跟踪
        var softDeletedEntities = this.ChangeTracker.Entries<ISoftDelete>()
            .Where(x => x.State == EntityState.Modified && x.Entity.IsDeleted)
            .Select(x => x.Entity)
            .ToList();
        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        softDeletedEntities.ForEach(x => this.Entry(x).State = EntityState.Detached);
        return result;
    }
}

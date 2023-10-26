using Commons.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Listening.Infrastructure;

public class ListeningDbContext : BaseDbContext
{
    public ListeningDbContext(DbContextOptions<ListeningDbContext> options, IMediator? mediator) : base(options, mediator)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        modelBuilder.EnableSoftDeletionGlobalFilter();
    }
}

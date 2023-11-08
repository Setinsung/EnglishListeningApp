using Commons.Infrastructure;
using MediaEncoder.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MediaEncoder.Infrastructure;

public class MediaEncoderDbContext : BaseDbContext
{
    public DbSet<EncodingItem> EncodingItems { get; private set; }

    public MediaEncoderDbContext(DbContextOptions options, IMediator? mediator) : base(options, mediator)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);  
        modelBuilder.EnableSoftDeletionGlobalFilter();
    }
}

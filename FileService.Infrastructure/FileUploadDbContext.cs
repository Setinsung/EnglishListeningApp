using Commons.Infrastructure;
using FileService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FileService.Infrastructure;

public class FileUploadDbContext : BaseDbContext
{
    public DbSet<UploadedItem> UploadedItems { get; set; }
    public FileUploadDbContext(DbContextOptions options, IMediator? mediator) : base(options, mediator)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
    }
}

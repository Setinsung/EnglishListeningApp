using Commons.CommonInitializer;
using FileService.Infrastructure;
using Microsoft.EntityFrameworkCore.Design;

namespace FileService.WebAPI;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FileUploadDbContext>
{
    public FileUploadDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = DbContextOptionsBuilderFactory.Create<FileUploadDbContext>();
        return new FileUploadDbContext(optionsBuilder.Options, null);
    }
}

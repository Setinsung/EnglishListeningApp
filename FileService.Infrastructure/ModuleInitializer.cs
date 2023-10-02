using Commons.Extensions;
using FileService.Domain;
using FileService.Infrastructure.ServiceImpls;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.Infrastructure;

public class ModuleInitializer : IModuleInitializer
{
    public void Initialize(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IStorageClient, SMBStorageClient>();
        services.AddScoped<IStorageClient, MockCloudStorageClient>();
        services.AddScoped<IFileServiceRepository, FileServiceRepository>();
        services.AddScoped<FileDomainService>();
        services.AddHttpClient();
    }
}

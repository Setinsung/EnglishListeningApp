using Commons.Helpers;
using MediaEncoder.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace MediaEncoder.Infrastructure;

public class ModuleInitializer : IModuleInitializer
{
    public void Initialize(IServiceCollection services)
    {
        services.AddScoped<IMediaEncoderRepository, MediaEncoderRepository>();
        services.AddScoped<IMediaEncoder, M4AEncoder>();
    }
}

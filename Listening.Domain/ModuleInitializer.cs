using Commons.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Listening.Domain;

public class ModuleInitializer : IModuleInitializer
{
    public void Initialize(IServiceCollection services)
    {
        services.AddScoped<ListeningDomainService>();
    }
}

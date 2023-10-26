using Commons.Helpers;
using Listening.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Listening.Infrastructure;

public class ModuleInitializer : IModuleInitializer
{
    public void Initialize(IServiceCollection services)
    {
        services.AddScoped<IListeningRepository, ListeningRepository>();
    }
}

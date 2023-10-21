using Commons.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Commons.JWT;

public class ModuleInitializer : IModuleInitializer
{
    public void Initialize(IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
    }
}
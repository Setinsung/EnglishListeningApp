using Microsoft.Extensions.DependencyInjection;

namespace Commons.Helpers;

public interface IModuleInitializer
{
    public void Initialize(IServiceCollection services);
}

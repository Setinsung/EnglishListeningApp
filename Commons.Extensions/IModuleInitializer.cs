using Microsoft.Extensions.DependencyInjection;

namespace Commons.Extensions;

public interface IModuleInitializer
{
    public void Initialize(IServiceCollection services);
}

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Commons.Helpers;

public static class ModuleInitializerExtensions
{
    /// <summary>
    /// 在给定的一组程序集中运行模块初始化器。
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="assemblies">要搜索的程序集集合</param>
    /// <returns>更新后的服务集合</returns>
    public static IServiceCollection RunModuleInitializers(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        foreach (var asm in assemblies)
        {
            Type[] types = asm.GetTypes();
            var moduleInitializerTypes = types.Where(t => !t.IsAbstract && typeof(IModuleInitializer).IsAssignableFrom(t));
            foreach (var implType in moduleInitializerTypes)
            {
                var initializer = (IModuleInitializer?)Activator.CreateInstance(implType);
                if(initializer == null) throw new ApplicationException($"Cannot create ${implType}");
                initializer.Initialize(services);
            }
        }
        return services;
    }
}

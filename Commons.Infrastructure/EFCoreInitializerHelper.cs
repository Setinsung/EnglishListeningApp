using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Commons.Infrastructure;

public static class EFCoreInitializerHelper
{
    /// <summary>
    /// 向服务集合中添加所有满足条件的 DbContext。
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="builder">配置 DbContextOptionsBuilder 的委托</param>
    /// <param name="assemblies">要搜索的程序集集合</param>
    /// <returns>更新后的服务集合</returns>
    public static IServiceCollection AddAllDbContext(this IServiceCollection services, Action<DbContextOptionsBuilder> builder, IEnumerable<Assembly> assemblies)
    {
        // AddDbContextPool不支持DbContext注入其他对象，而且使用不当有内存暴涨的问题，因此不使用
        Type[] types = new Type[] { typeof(IServiceCollection), typeof(Action<DbContextOptionsBuilder>), typeof(ServiceLifetime), typeof(ServiceLifetime) };
        var methodAddDbContext = typeof(EntityFrameworkServiceCollectionExtensions)
            .GetMethod(nameof(EntityFrameworkServiceCollectionExtensions.AddDbContext), 1, types);
        foreach(var asmToLoad in assemblies)
        {
            var dbCtxTypes = asmToLoad.GetTypes().Where(t => !t.IsAbstract && typeof(DbContext).IsAssignableFrom(t));
            foreach (var dbCtxType in dbCtxTypes)
            {
                // 先将DbContext作为泛型参数传递给方法再Invoke调用，这是静态方法不用传入实例，只需传入object参数数组
                var methodGenericAddDbContext = methodAddDbContext.MakeGenericMethod(dbCtxType);
                methodGenericAddDbContext.Invoke(null, new object[] { services, builder, ServiceLifetime.Scoped, ServiceLifetime.Scoped });
            }
        }
        return services;
    }
}

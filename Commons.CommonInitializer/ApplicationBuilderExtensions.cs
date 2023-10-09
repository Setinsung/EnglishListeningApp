using Commons.EventBus;
using Microsoft.AspNetCore.Builder;

namespace Commons.CommonInitializer;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCommonDefault(this IApplicationBuilder app)
    {
        app.UseEventBus();
        app.UseCors();
        app.UseForwardedHeaders(); // Nginx转发header
        // app.UseHttpsRedirection(); // 与ForwardedHeaders一起使用有冲突，且WebAPI项目不需要配置
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }

}

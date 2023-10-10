using Commons.EventBus;
using Commons.Helpers;
using Commons.Helpers.JsonConverters;
using Commons.Infrastructure;
using Commons.JWT;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Commons.CommonInitializer;

public static class WebApplicationBuilderExtensions
{
    public static void ConfigureDbConfiguration(this WebApplicationBuilder builder)
    {
        builder.Host.ConfigureAppConfiguration((hostCtx, configBuilder) =>
        {
            string connStr = builder.Configuration.GetConnectionString("Default");
            configBuilder.AddDbConfiguration(() => new SqlConnection(connStr), reloadOnChange: true, reloadInterval: TimeSpan.FromSeconds(5));
        });
    }

    public static void ConfigureExtraServices(this WebApplicationBuilder builder, InitializerOptions initOptions)
    {
        IServiceCollection services = builder.Services;
        IConfiguration configuration = builder.Configuration;
        var assemblies = ReflectionHelper.GetAllReferencedAssemblies();
        
        // 运行所有的ModuleInitializer
        services.RunModuleInitializers(assemblies);
        
        // 注册所有的DbContext
        services.AddAllDbContext(ctx =>
        {
            string connStr = builder.Configuration.GetConnectionString("Default");
            ctx.UseSqlServer(connStr);
        }, assemblies);

        // 配置授权和鉴权，JWT，swagger可携带JWT
        // 只要需要校验Authentication报文头，即使非IdentityService.WebAPI项目也要启用，IdentityService项目还需要启用AddIdentityCore
        services.AddAuthorization();
        services.AddAuthentication();
        JWTOptions jwtOpt = configuration.GetSection("JWT").Get<JWTOptions>();
        services.AddJWTAuthentication(jwtOpt);
        services.Configure<JWTOptions>(configuration.GetSection("JWT"));
        services.Configure<SwaggerGenOptions>(c => c.AddAuthenticationHeader());

        // mediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies.ToArray()));

        // 日期Json解析配置
        services.Configure<JsonOptions>(opt =>
        {
            // 设置时间格式。而非“2008-08-08T08:08:08”的数据库读出的格式
            opt.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
        });

        // Cors
        services.AddCors(opt =>
        {
            var corsOpt = configuration.GetSection("Cors").Get<CorsOptions>();
            string[] urls = corsOpt.Origins;
            opt.AddDefaultPolicy(builder => builder
                .WithOrigins(urls)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );
        });

        // logging
        services.AddLogging(builder =>
        {
            Log.Logger = new LoggerConfiguration()
                // .MinimumLevel.Information().Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(initOptions.LogFilePath)
                .CreateLogger();
            builder.AddSerilog();
        });

        // EventBus
        services.Configure<IntegrationEventRabbitMQOptions>(configuration.GetSection("RabbitMQ"));
        services.AddEventBus(initOptions.EventBusQueueName, assemblies);

        // FluentValidation
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblies(assemblies);

        // Redis连接复用器
        string redisConnStr = configuration.GetValue<string>("Redis:ConnStr");
        IConnectionMultiplexer redisConnMultiplexer = ConnectionMultiplexer.Connect(redisConnStr);
        services.AddSingleton(typeof(IConnectionMultiplexer), redisConnMultiplexer);
        
        // Nginx转发配置
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.All;
        });
    }
}

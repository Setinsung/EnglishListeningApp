using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Reflection;

namespace Commons.EventBus;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddEventBus(this IServiceCollection services, string queueName, params Assembly[] assemblies)
    {
        return AddEventBus(services, queueName, assemblies.ToList());
    }

    public static IServiceCollection AddEventBus(this IServiceCollection services, string queueName, IEnumerable<Assembly> assemblies)
    {
        List<Type> eventHandlers = new();
        foreach (var asm in assemblies)
        {
            var types = asm.GetTypes().Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(IIntegrationEventHandler)));
            eventHandlers.AddRange(types);
        }
        return AddEventBus(services, queueName, eventHandlers);
    }

    public static IServiceCollection AddEventBus(this IServiceCollection services, string queueName, List<Type> eventHandlers)
    {
        foreach (var type in eventHandlers)
        {
            services.AddScoped(type, type);
        }
        services.AddSingleton<IEventBus>(sp => // 注册服务时就要读取配置，使用AddSingleton的Func<IServiceProvider, TService>重载，可以直接拿到IServiceProvider，无须自己构建IServiceProvider
        {
            var optionMQ = sp.GetRequiredService<IOptions<IntegrationEventRabbitMQOptions>>().Value;
            var factory = new ConnectionFactory()
            {
                HostName = optionMQ.HostName,
                DispatchConsumersAsync = true,
            };
            if (optionMQ.UserName != null)
            {
                factory.UserName = optionMQ.UserName;
            }
            if (optionMQ.Password != null)
            {
                factory.Password = optionMQ.Password;
            }
            // eventBus归DI管理，释放的时候会调用Dispose，eventbus的Dispose中会销毁RabbitMQConnection
            RabbitMQConnection mqConnection = new RabbitMQConnection(factory);
            var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
            var eventBus = new RabbitMQEventBus(mqConnection, serviceScopeFactory, optionMQ.ExchangeName, queueName);
            // 遍历所有实现了IIntegrationEventHandler接口的类，批量注册到eventBus
            foreach (var type in eventHandlers)
            {
                // 获取类上标注的EventNameAttribute，EventNameAttribute的Name为要监听的事件的名字
                // 允许监听多个事件，但是不能为空
                var eventNameAttrs = type.GetCustomAttributes<EventNameAttribute>();
                if(!eventNameAttrs.Any()) throw new ApplicationException($"There shoule be at least one EventNameAttribute on {type}");
                foreach (var eventNameAttr in eventNameAttrs)
                {
                    eventBus.Subscribe(eventNameAttr.Name, type);
                }
            }

            return eventBus;
        });
        return services;
    }
}

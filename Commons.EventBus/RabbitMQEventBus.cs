using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Commons.EventBus;

public class RabbitMQEventBus : IEventBus, IDisposable
{
    private readonly RabbitMQConnection _persistentConnection;
    private readonly SubscriptionsManager _subsManager;
    private readonly IModel _consumerChannel;
    private readonly string _exchangeName;
    private string _queueName;
    private readonly IServiceScope _serviceScope;

    public RabbitMQEventBus(RabbitMQConnection persistentConnection, IServiceScopeFactory serviceScopeFactory, string exchangeName, string queueName)
    {
        this._persistentConnection = persistentConnection ?? throw new ArgumentException(nameof(persistentConnection));
        this._subsManager = new SubscriptionsManager();
        this._exchangeName = exchangeName;
        this._queueName = queueName;
        // IIntegrationEventHandler以及其注册的服务都是Scoped，而RabbitMQEventBus是Singletion，此处显示创建scope
        this._serviceScope = serviceScopeFactory.CreateScope();
        this._consumerChannel = CreateConsumerChannel();
        this._subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
    }


    public void Publish(string eventName, object? eventData)
    {
        if (!_persistentConnection.IsConnected) _persistentConnection.TryConnect();
        // Connection 可以创建多个Channel ，Channel不是线程安全的所以不能在线程间共享。
        using var channel = _persistentConnection.CreateModel();
        channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct);
        byte[] body;
        if (eventData == null) body = new byte[0];
        else
        {
            JsonSerializerOptions options = new() { WriteIndented = true };
            body = JsonSerializer.SerializeToUtf8Bytes(eventData, eventData.GetType(), options);
        }
        var properties = channel.CreateBasicProperties();
        properties.DeliveryMode = 2;

        channel.BasicPublish(
            exchange: _exchangeName,
            routingKey: eventName,
            mandatory: true,
            basicProperties: properties,
            body: body
        );
    }

    public void Subscribe(string eventName, Type handlerType)
    {
        CheckHandlerType(handlerType);
        DoInternalSubscription(eventName);
        _subsManager.AddSubscription(eventName, handlerType);
        StartBasicConsume();
    }

    public void Unsubscribe(string eventName, Type handlerType)
    {
        CheckHandlerType(handlerType);
        _subsManager.RemoveSubscription(eventName, handlerType);
    }

    public void Dispose()
    {
        if (_consumerChannel != null) _consumerChannel.Dispose();
        _subsManager.Clear();
        _persistentConnection.Dispose();
        _serviceScope.Dispose();
    }

    private void CheckHandlerType(Type handlerType) // 检查handlerType是否实现IIntegrationEventHandler接口
    {
        if (!typeof(IIntegrationEventHandler).IsAssignableFrom(handlerType))
        {
            throw new ArgumentException($"{handlerType} doesn't inherit from IIntegrationEventHandler", nameof(handlerType));
        }
    }

    private void DoInternalSubscription(string eventName)
    {
        if (_subsManager.HasSubscriptionsForEvent(eventName)) return;
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }
        _consumerChannel.QueueBind(
            queue: _queueName,
            exchange: _exchangeName,
            routingKey: eventName
        );
    }

    private void StartBasicConsume() // 开始消费信息
    {
        if (_consumerChannel == null) return;
        var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
        consumer.Received += Comsumer_Received;
        _consumerChannel.BasicConsume(
            queue: _queueName,
            autoAck: false, // 不自动应答
            consumer: consumer
        );
    }

    private async Task Comsumer_Received(object sender, BasicDeliverEventArgs eventArgs) // 信息接收
    {
        var eventName = eventArgs.RoutingKey; // 此处eventName就是routingKey
        var message = Encoding.UTF8.GetString(eventArgs.Body.Span);// 要求所有的消息都是字符串的json
        try
        {
            await ProcessEvent(eventName, message);
            // 如果在获取消息时采用不自动应答，但获取消息后又不调用basicAck，RabbitMQ会认为消息没有投递成功。不仅所有的消息都会保留到内存中，而且在客户重新连接后，会将消息重新投递一遍。这种情况无法完全避免，因此EventHandler的代码需要幂等
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false); // multiple：批量确认标志。如果值为true，则执行批量确认，此deliveryTag之前收到的消息全部进行确认
        }
        catch (Exception ex)
        {
            // _consumerChannel.BasicReject(eventArgs.DeliveryTag, true);
            Debug.Fail(ex.ToString());
        }
    }

    private async Task ProcessEvent(string eventName, string message) // 具体由eventName的所有监听者收取消息
    {
        if (!_subsManager.HasSubscriptionsForEvent(eventName))
        {
            string entryAsm = Assembly.GetEntryAssembly().GetName().Name;
            Debug.WriteLine($"找不到可以处理eventName={eventName}的处理程序，entryAsm:{entryAsm}");
        }
        else
        {
            var subscriptions = _subsManager.GetHandlersForEvent(eventName);
            foreach (var subscription in subscriptions)
            {
                // 各自在不同的Scope中，避免DbContext等的共享造成问题
                IIntegrationEventHandler? handler = _serviceScope.ServiceProvider.GetService(subscription) as IIntegrationEventHandler;
                if (handler == null) throw new ApplicationException($"无法创建{subscription}类型的服务");
                await handler.Handle(eventName, message);
            }
        }
    }

    private IModel CreateConsumerChannel() // 创建消费者信道连接
    {
        if (!_persistentConnection.IsConnected) _persistentConnection.TryConnect();
        var channel = _persistentConnection.CreateModel();
        channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct);
        channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        channel.CallbackException += (sender, ea) => // 消息处理失败
        {
            Debug.Fail(ea.ToString());
        };
        return channel;
    }

    private void SubsManager_OnEventRemoved(object? sender, string eventName) // 事件移除时，取消交换机对队列的绑定
    {
        if (!_persistentConnection.IsConnected) _persistentConnection.TryConnect();
        using var channel = _persistentConnection.CreateModel();
        channel.QueueUnbind(
            queue: _queueName,
            exchange: _exchangeName,
            routingKey: eventName
        );
        if (_subsManager.IsEmpty)
        {
            _queueName = string.Empty;
            _consumerChannel.Close();
        }
    }


}

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Commons.EventBus;

public class RabbitMQConnection
{
    private readonly IConnectionFactory _connectionFactory;
    private IConnection _connection;
    private bool _disposed;
    private readonly object sync_root = new object();

    public bool IsConnected { get { return _connection != null && _connection.IsOpen && !_disposed; } }

    public RabbitMQConnection(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public IModel CreateModel()
    {
        if (!IsConnected) throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
        return _connection.CreateModel();
    }

    public void Dispose()
    {
        if(_disposed) return;
        _disposed = true;
        _connection.Dispose();
    }

    public bool TryConnect()
    {
        lock (sync_root) // 互斥锁，同一时间只有一个线程可以执行连接操作
        {
            _connection = _connectionFactory.CreateConnection();
            if (!IsConnected) return false;
            // 阻塞、异常、关闭 重新连接
            _connection.ConnectionShutdown += OnConnectionShutdown;
            _connection.CallbackException += OnCallbackException;
            _connection.ConnectionBlocked += OnConnectionBlocked;
            return true;
        }
    }

    private void OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
        if (_disposed) return;
        TryConnect();
    }

    private void OnCallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        if (_disposed) return;
        TryConnect();
    }

    private void OnConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        if (_disposed) return;
        TryConnect();
    }

}

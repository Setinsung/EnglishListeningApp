namespace Commons.EventBus;
/// <summary>
/// 提供事件处理的注册和事件的分发机制
/// </summary>
public class SubscriptionsManager
{
    private readonly Dictionary<string, List<Type>> _handlers = new();
    public event EventHandler<string>? OnEventRemoved;
    public bool IsEmpty => !_handlers.Keys.Any();
    public void Clear() => _handlers.Clear();

    /// <summary>
    /// 把eventHandlerType类型（实现了eventHandlerType接口的）注册为eventName事件的监听者
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="eventHandlerType"></param>
    public void AddSubscription(string eventName, Type eventHandlerType)
    {
        if (!HasSubscriptionsForEvent(eventName))
        {
            _handlers.Add(eventName, new List<Type>());
        }
        if (_handlers[eventName].Contains(eventHandlerType)) // 已添加监听者重复报错
        {
            throw new ArgumentException($"Handler Type {eventHandlerType} already registered for '{eventName}'", nameof(eventHandlerType));
        }
        _handlers[eventName].Add(eventHandlerType);
    }

    /// <summary>
    /// 移除对eventName事件的指定监听者
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="eventHandlerType"></param>
    public void RemoveSubscription(string eventName, Type eventHandlerType)
    {
        _handlers[eventName].Remove(eventHandlerType);
        if (!_handlers[eventName].Any()) // 如果事件无监听者了，那么再把事件移除，触发回调
        {
            _handlers.Remove(eventName);
            OnEventRemoved?.Invoke(this, eventName);
        }
    }

    /// <summary>
    /// 得到名字为eventName的所有监听者
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    public IEnumerable<Type> GetHandlersForEvent(string eventName) => _handlers[eventName];

    /// <summary>
    /// 是否有类型监听eventName事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);
}

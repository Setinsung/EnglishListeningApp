namespace Commons.EventBus;

public interface IEventBus
{
    /// <summary>
    /// 发布事件，eventName指定事件名也是routingKey值，eventData事件对象
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="eventData"></param>
    void Publish(string eventName, object? eventData);

    /// <summary>
    /// 订阅事件，eventName指定事件名也是routingKey值，handlerType指定此事件的监听类
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="handlerType"></param>
    void Subscribe(string eventName, Type handlerType);

    /// <summary>
    /// 取消订阅事件，eventName指定事件名也是routingKey值，handlerType指定此事件的监听类
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="handlerType"></param>
    void Unsubscribe(string eventName, Type handlerType);
}

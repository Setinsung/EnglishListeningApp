using Dynamic.Json;

namespace Commons.EventBus;

public abstract class DynamicIntegrationEventHandler : IIntegrationEventHandler
{
    public Task Handle(string eventName, string eventData)
    {
        // Json反序列化为dynamic
        dynamic dynamicEventData = DJson.Parse(eventData);
        return HandleDynamic(eventName, dynamicEventData);
    }

    public abstract Task HandleDynamic(string eventName, dynamic eventData);
}

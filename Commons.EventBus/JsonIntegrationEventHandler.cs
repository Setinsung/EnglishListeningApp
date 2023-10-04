using System.Text.Json;

namespace Commons.EventBus;

public abstract class JsonIntegrationEventHandler<T> : IIntegrationEventHandler
{
    public Task Handle(string eventName, string eventData)
    {
        T? eventObj = JsonSerializer.Deserialize<T>(eventData);
        return HandleJson(eventName, eventObj);
    }

    public abstract Task HandleJson(string eventName, T? eventObj);
}

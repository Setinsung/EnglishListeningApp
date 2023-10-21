using Commons.EventBus;
using IdentityService.Domain;

namespace IdentityService.WebAPI.Events;

public class UserCreatedEventHandler : JsonIntegrationEventHandler<UserCreatedEvent>
{
    private readonly ILogger<UserCreatedEventHandler> _logger;
    private readonly ISmsSender _smsSender;

    public UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger,ISmsSender smsSender)
    {
        this._logger = logger;
        this._smsSender = smsSender;
    }
    public override Task HandleJson(string eventName, UserCreatedEvent? eventObj)
    {
        _logger.LogInformation(eventName, eventObj);
        return _smsSender.SendAsync(eventObj.PhoneNum, eventObj.Password);
    }
}

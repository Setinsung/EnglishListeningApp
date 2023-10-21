using Commons.EventBus;
using IdentityService.Domain;

namespace IdentityService.WebAPI.Events;

[EventName("IdentityService.User.PasswordReset")]
public class ResetPasswordEventHandler : JsonIntegrationEventHandler<ResetPasswordEvent>
{
    private readonly ILogger<ResetPasswordEventHandler> _logger;
    private readonly ISmsSender _smsSender;

    public ResetPasswordEventHandler(ILogger<ResetPasswordEventHandler> logger, ISmsSender smsSender)
    {
        this._logger = logger;
        this._smsSender = smsSender;
    }
    public override Task HandleJson(string eventName, ResetPasswordEvent? eventObj)
    {
        _logger.LogInformation(eventName, eventObj);
        return _smsSender.SendAsync(eventObj.PhoneNum, eventObj.Password);
    }
}

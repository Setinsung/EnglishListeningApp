namespace IdentityService.WebAPI.Controllers.Events;

public record UserCreatedEvent(Guid Id, string UserName, string Password, string PhoneNum);

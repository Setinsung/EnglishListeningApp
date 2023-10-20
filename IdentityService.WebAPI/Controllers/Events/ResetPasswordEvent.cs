namespace IdentityService.WebAPI.Controllers.Events;

public record ResetPasswordEvent(Guid Id, string UserName, string Password, string PhoneNum);


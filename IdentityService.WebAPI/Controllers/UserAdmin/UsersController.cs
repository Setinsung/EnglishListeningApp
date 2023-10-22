using Commons.EventBus;
using IdentityService.Domain;
using IdentityService.Infrastructure;
using IdentityService.WebAPI.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.WebAPI.Controllers.UserAdmin;

[Route("[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IdUserManager _userManager;
    private readonly IEventBus _eventBus;
    private readonly IIdentityServiceRepository _identityServiceRepository;

    public UsersController(IdUserManager userManager, IEventBus eventBus, IIdentityServiceRepository identityServiceRepository)
    {
        this._userManager = userManager;
        this._eventBus = eventBus;
        this._identityServiceRepository = identityServiceRepository;
    }

    [HttpGet]
    public Task<UserDTO[]> FindAllUsers()
    {
        return _userManager.Users.Select(u => UserDTO.Create(u)).ToArrayAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> FindById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if(user == null) return NotFound("用户没找到");
        return UserDTO.Create(user);
    }

    [HttpPost]
    public async Task<ActionResult> AddAdminUser(AddAdminUserRequest req)
    {
        (var identityResult, var user, string? password) = await _identityServiceRepository.AddAdminUserAsync(req.UserName, req.PhoneNum);
        if (!identityResult.Succeeded) return BadRequest(identityResult.Errors.SumErrors());
        // 发布事件
        var userCreatedEvent = new UserCreatedEvent(user.Id, req.UserName, password, req.PhoneNum);
        _eventBus.Publish("IdentityService.User.Created", userCreatedEvent);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAdminUser(string id)
    {
        var identityResult = await _identityServiceRepository.RemoveUserAsync(id);
        if (!identityResult.Succeeded) return NotFound("用户没找到");
        else return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAdminUser(string id, EditAdminUserRequest req)
    {
        var user = await _identityServiceRepository.FindByIdAsync(id);
        if (user == null) return NotFound("用户没找到");
        user.PhoneNumber = req.PhoneNum;
        await _userManager.UpdateAsync(user);
        return Ok();
    }

    [HttpPut("{id}/password")]
    public async Task<ActionResult> RsetAdminUserPassword(string id)
    {
        (var resetPwdRs, var user, var password) = await _identityServiceRepository.ResetPasswordAsync(id);
        if (!resetPwdRs.Succeeded) return BadRequest(resetPwdRs.Errors.SumErrors());
        // 发布事件
        var eventData = new ResetPasswordEvent(user.Id, user.UserName, password, user.PhoneNumber);
        _eventBus.Publish("IdentityService.User.PasswordReset", eventData);
        return Ok();
    }
}

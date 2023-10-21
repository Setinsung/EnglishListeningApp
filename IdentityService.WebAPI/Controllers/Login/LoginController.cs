using IdentityService.Domain;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace IdentityService.WebAPI.Controllers.Login;

[Route("[controller]/[action]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IdDomainService _idDomainService;
    private readonly IIdentityServiceRepository _identityServiceRepository;

    public LoginController(IdDomainService idDomainService, IIdentityServiceRepository identityServiceRepository)
    {
        this._idDomainService = idDomainService;
        this._identityServiceRepository = identityServiceRepository;
    }

    [HttpPost]
    public async Task<ActionResult> CreateWorld()
    {
        bool ok = await _idDomainService.CreateWorld();
        if (!ok)
            return StatusCode((int)HttpStatusCode.Conflict, "已经初始化过了");
        else
            return Ok();
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<UserResponse>> GetUserInfo()
    {
        string? userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return NotFound();
        User? user = await _identityServiceRepository.FindByIdAsync(userId);
        if (user == null) return NotFound();
        return new UserResponse(user.Id, user.PhoneNumber, user.CreatedTime);
    }

    [HttpPost]
    public async Task<ActionResult<string?>> LoginByPhoneAndPwd(LoginByPhoneAndPwdRequest req)
    {
        (var checkResult, string? token) = await _idDomainService.LoginByPhoneAndPwdAsync(req.PhoneNum, req.Password);
        return checkResult switch
        {
            { Succeeded: true } => token,
            { IsLockedOut: true } => StatusCode((int)HttpStatusCode.Locked, "此账号已经锁定"),
            _ => StatusCode((int)HttpStatusCode.BadRequest, "登录失败: " + checkResult.ToString())
        };
    }

    [HttpPost]
    public async Task<ActionResult<string?>> LoginByUserNameAndPwd(
    LoginByUserNameAndPwdRequest req)
    {
        (var checkResult, string? token) = await _idDomainService.LoginByUserNameAndPwdAsync(req.UserName, req.Password);
        return checkResult switch
        {
            { Succeeded: true } => token,
            { IsLockedOut: true } => StatusCode((int)HttpStatusCode.Locked, "此账号已经锁定"),
            _ => StatusCode((int)HttpStatusCode.BadRequest, "登录失败: " + checkResult.ToString())
        };
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> ChangeMyPassword(ChangeMyPasswordRequest req)
    {
        string? userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return NotFound();
        var changeePwdRs = await _identityServiceRepository.ChangePasswordAsync(userId, req.Password);
        if (changeePwdRs.Succeeded) return Ok();
        else return BadRequest(changeePwdRs.Errors.SumErrors());
    }
}

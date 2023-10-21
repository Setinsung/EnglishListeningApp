using Commons.JWT;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Claims;

namespace IdentityService.Domain;

public class IdDomainService
{
    private readonly IIdentityServiceRepository _identityServiceRepository;
    private readonly ITokenService _tokenService;
    private readonly IOptions<JWTOptions> _jwtOptions;

    public IdDomainService(IIdentityServiceRepository identityServiceRepository, ITokenService tokenService, IOptions<JWTOptions> jwtOptions)
    {
        this._identityServiceRepository = identityServiceRepository;
        this._tokenService = tokenService;
        this._jwtOptions = jwtOptions;
    }

    /// <summary>
    /// 使用手机号和密码进行登录
    /// </summary>
    /// <param name="phoneNum">手机号</param>
    /// <param name="password">密码</param>
    /// <returns>登录结果和令牌</returns>
    public async Task<(SignInResult signInResult, string? token)> LoginByPhoneAndPwdAsync(string phoneNum, string password)
    {
        var (checkResult, user) = await this.CheckPhoneNumAndPwdAsync(phoneNum, password);
        string? token = null;
        if (checkResult.Succeeded && user != null)
        {
            token = await this.BuildTokenAsync(user);
        }
        return (checkResult, token);
    }

    /// <summary>
    /// 使用用户名和密码进行登录
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="password">密码</param>
    /// <returns>登录结果和用户</returns>
    public async Task<(SignInResult signInResult, string? user)> LoginByUserNameAndPwdAsync(string userName, string password)
    {
        var (checkResult , user)  = await this.CheckUserNameAndPwdAsync(userName, password);
        string? token = null;
        if(checkResult.Succeeded && user != null)
        {
            token = await this.BuildTokenAsync(user);
        }
        return (checkResult, token);
    }
    /// <summary>
    /// 创建admin用户
    /// </summary>
    /// <returns></returns>
    public async Task<bool> CreateWorld()
    {
        if (await _identityServiceRepository.FindByNameAsync("admin") != null) return false;
        string userName = "admin";
        string pwd = "123456";
        string phoneNum = "18918999999";
        User user = new(userName);
        IdentityResult createRs = await _identityServiceRepository.CreateAsync(user, pwd);
        Debug.Assert(createRs.Succeeded);
        string token = await _identityServiceRepository.GenerateChangePhoneNumberTokenAsync(user, phoneNum);
        SignInResult changePhoneNumRs = await _identityServiceRepository.ChangePhoneNumAsync(user.Id.ToString(), phoneNum, token);
        Debug.Assert(changePhoneNumRs.Succeeded);
        IdentityResult addToRoleRs = await _identityServiceRepository.AddToRoleAsync(user, "User");
        Debug.Assert(addToRoleRs.Succeeded);
        addToRoleRs = await _identityServiceRepository.AddToRoleAsync(user, "Admin");
        Debug.Assert(addToRoleRs.Succeeded);
        return true;
    }

    private async Task<string> BuildTokenAsync(User user)
    {
        var roles = await _identityServiceRepository.GetRolesAsync(user);
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        return _tokenService.BuildToken(claims, _jwtOptions.Value);
    }

    private async Task<(SignInResult checkResult, User? user)> CheckPhoneNumAndPwdAsync(string phoneNum, string password)
    {
        SignInResult checkResult = SignInResult.Failed;
        var user = await _identityServiceRepository.FindByPhoneNumberAsync(phoneNum);
        if (user != null)
        {
            checkResult = await _identityServiceRepository.CheckForSignInAsync(user, password, true);
        }
        return (checkResult, user);
    }

    private async Task<(SignInResult checkResult, User? user)> CheckUserNameAndPwdAsync(string userName, string password)
    {
        SignInResult checkResult = SignInResult.Failed;
        var user = await _identityServiceRepository.FindByNameAsync(userName);
        if (user != null)
        {
            checkResult = await _identityServiceRepository.CheckForSignInAsync(user,password, true);
        }
        return (checkResult, user);
    }
}

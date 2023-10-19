using IdentityService.Domain;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

namespace IdentityService.Infrastructure;

public class IdentityServiceRepository : IIdentityServiceRepository
{
    private readonly IdUserManager _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly ILogger<IdentityServiceRepository> _logger;

    public IdentityServiceRepository(IdUserManager userManager, RoleManager<Role> roleManager, ILogger<IdentityServiceRepository> logger)
    {
        this._userManager = userManager;
        this._roleManager = roleManager;
        this._logger = logger;
    }

    public Task<IdentityResult> AccessFailedAsync(User user)
    {
        return _userManager.AccessFailedAsync(user);
    }

    public async Task<(IdentityResult identityResult, User? user, string? password)> AddAdminUserAsync(string userName, string phoneNum)
    {
        if (await FindByNameAsync(userName) != null) return (ErrorResult($"已存在用户名{userName}"), null, null);
        if (await FindByPhoneNumberAsync(phoneNum) != null) return (ErrorResult($"已存在手机号{phoneNum}"), null, null);
        User user = new(userName)
        {
            PhoneNumber = phoneNum,
            PhoneNumberConfirmed = true,
        };
        string password = GeneratePassword();
        var result = await CreateAsync(user, password);
        if (!result.Succeeded) return (result, null, null);
        result = await AddToRoleAsync(user, "Admin");
        if (!result.Succeeded) return (result, null, null);
        return (result, user, password);
    }

    public async Task<IdentityResult> AddToRoleAsync(User user, string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            Role role = new() { Name = roleName };
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded) return result;
        }
        return await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<IdentityResult> ChangePasswordAsync(Guid userId, string password)
    {
        if (password.Length < 6)
            return IdentityResult.Failed(new IdentityError()
            {
                Code = "Password Invalid",
                Description = "密码长度不能小于6"
            });
        User user = await FindByIdOrThrowAsync(userId);
        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
        IdentityResult resetPwdResult = await _userManager.ResetPasswordAsync(user, token, password);
        return resetPwdResult;
    }

    public async Task<SignInResult> ChangePhoneNumAsync(Guid userId, string phoneNum, string token)
    {
        User user = await FindByIdOrThrowAsync(userId);
        IdentityResult changeResult = await _userManager.ChangePhoneNumberAsync(user, phoneNum, token);
        if (!changeResult.Succeeded)
        {
            await _userManager.AccessFailedAsync(user);
            string errMsg = changeResult.Errors.SumErrors();
            _logger.LogWarning("{phoneNum} ChangePhoneNumberAsync失败，错误信息: {errMsg}", phoneNum, errMsg);
            return SignInResult.Failed;
        }
        else
        {
            await ConfirmPhoneNumberAsync(user.Id);
            return SignInResult.Success;
        }
    }

    public async Task<SignInResult> CheckForSignInAsync(User user, string password, bool lockoutOnFailure)
    {
        if (await _userManager.IsLockedOutAsync(user)) return SignInResult.LockedOut;
        if (await _userManager.CheckPasswordAsync(user, password)) return SignInResult.Success;
        else
        {
            if (lockoutOnFailure)
            {
                IdentityResult identityResult = await _userManager.AccessFailedAsync(user);
                if (!identityResult.Succeeded) throw new ApplicationException("AccessFailed failed");
            }
            return SignInResult.Failed;
        }

    }

    public async Task ConfirmPhoneNumberAsync(Guid userId)
    {
        User user = await FindByIdOrThrowAsync(userId);
        user.PhoneNumberConfirmed = true;
        await _userManager.UpdateAsync(user);
    }

    public Task<IdentityResult> CreateAsync(User user, string password)
    {
        return _userManager.CreateAsync(user, password);
    }

    public Task<User?> FindByIdAsync(Guid userId)
    {
        return _userManager.FindByIdAsync(userId.ToString());
    }



    public Task<User?> FindByNameAsync(string userName)
    {
        return _userManager.FindByNameAsync(userName);
    }

    public Task<User?> FindByPhoneNumberAsync(string phoneNum)
    {
        return _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNum);
    }

    public Task<string> GenerateChangePhoneNumberTokenAsync(User user, string phoneNum)
    {
        return _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNum);
    }

    public Task<IList<string>> GetRolesAsync(User user)
    {
        return _userManager.GetRolesAsync(user);
    }

    public async Task<IdentityResult> RemoveUserAsync(Guid userId)
    {
        User user = await FindByIdOrThrowAsync(userId);
        var userLoginStore = _userManager.UserLoginStore;
        CancellationToken noneCT = default;
        var logins = await userLoginStore.GetLoginsAsync(user, noneCT);
        foreach (var login in logins)
        {
            await userLoginStore.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey, noneCT);
        }
        user.SoftDelete();
        IdentityResult result = await _userManager.UpdateAsync(user);
        return result;
    }

    public async Task<(IdentityResult identityResult, User? user, string? password)> ResetPasswordAsync(Guid userId)
    {
        User user = await FindByIdOrThrowAsync(userId);
        string password = GeneratePassword();
        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
        IdentityResult identityResult = await _userManager.ResetPasswordAsync(user, token, password);
        if(!identityResult.Succeeded) return (identityResult,  null, null);
        return (identityResult, user, password);
    }

    public async Task UpdatePhoneNumberAsync(Guid userId, string phoneNum)
    {
        User user = await FindByIdOrThrowAsync(userId);
        user.PhoneNumber = phoneNum;
        await _userManager.UpdateAsync(user);
    }

    /// <summary>
    /// 创建一个表示身份验证错误的 IdentityResult 对象。
    /// </summary>
    /// <param name="msg">错误消息的描述。</param>
    /// <returns>表示身份验证错误的 IdentityResult 对象。</returns>
    private static IdentityResult ErrorResult(string msg)
    {
        IdentityError idError = new() { Description = msg };
        return IdentityResult.Failed(idError);
    }

    /// <summary>
    /// 生成一个随机密码。
    /// </summary>
    /// <remarks>
    /// 该方法生成一个满足密码策略要求的随机密码，自动根据用户管理器的密码选项来确定密码的长度和要求的字符类型。
    /// </remarks>
    /// <returns>生成的随机密码。</returns>
    private string GeneratePassword()
    {
        var options = _userManager.Options.Password;
        int length = options.RequiredLength;
        bool nonAlphanumeric = options.RequireNonAlphanumeric;
        bool digit = options.RequireDigit;
        bool lowercase = options.RequireLowercase;
        bool uppercase = options.RequireUppercase;
        StringBuilder password = new();
        Random random = new();
        while (password.Length < length)
        {
            char c = (char)random.Next(32, 126);
            password.Append(c);
            if (char.IsDigit(c))
                digit = false;
            else if (char.IsLower(c))
                lowercase = false;
            else if (char.IsUpper(c))
                uppercase = false;
            else if (!char.IsLetterOrDigit(c))
                nonAlphanumeric = false;
        }

        if (nonAlphanumeric)
            password.Append((char)random.Next(33, 48));
        if (digit)
            password.Append((char)random.Next(48, 58));
        if (lowercase)
            password.Append((char)random.Next(97, 123));
        if (uppercase)
            password.Append((char)random.Next(65, 91));
        return password.ToString();
    }

    /// <summary>
    /// 根据用户ID查找用户，如果找不到则抛出异常。
    /// </summary>
    /// <param name="userId">要查找的用户ID。</param>
    /// <returns>找到的用户。</returns>
    /// <exception cref="ArgumentException">当找不到指定ID的用户时抛出。</exception>
    private async Task<User> FindByIdOrThrowAsync(Guid userId)
    {
        return await FindByIdAsync(userId)
            ?? throw new ArgumentException($"{userId}的用户不存在");
    }

}

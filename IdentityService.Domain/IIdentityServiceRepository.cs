﻿using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain;

/// <summary>
/// 身份服务仓储接口
/// </summary>
public interface IIdentityServiceRepository
{
    /// <summary>
    /// 根据Id获取用户
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <returns>用户实体</returns>
    Task<User?> FindByIdAsync(Guid userId);

    /// <summary>
    /// 根据用户名获取用户
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <returns>用户实体</returns>
    Task<User?> FindByNameAsync(string userName);

    /// <summary>
    /// 根据手机号获取用户
    /// </summary>
    /// <param name="phoneNum">手机号</param>
    /// <returns>用户实体</returns>
    Task<User?> FindByPhoneNumberAsync(string phoneNum);

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="password">密码</param>
    /// <returns>身份验证结果</returns>
    Task<IdentityResult> CreateAsync(User user, string password);

    /// <summary>
    /// 记录一次登录失败
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <returns>身份验证结果</returns>
    Task<IdentityResult> AccessFailedAsync(User user); 

    /// <summary>
    /// 生成更改手机号的令牌
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="phoneNum">手机号</param>
    /// <returns>令牌</returns>
    Task<string> GenerateChangePhoneNumberTokenAsync(User user, string phoneNum);

    /// <summary>
    /// 检测token，正确则更改手机号
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <param name="phoneNum">手机号</param>
    /// <param name="token">令牌</param>
    /// <returns>登录结果</returns>
    Task<SignInResult> ChangePhoneNumAsync(Guid userId, string phoneNum, string token);

    /// <summary>
    /// 更改密码
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <param name="password">密码</param>
    /// <returns>身份验证结果</returns>
    Task<IdentityResult> ChangePasswordAsync(Guid userId, string password);

    /// <summary>
    /// 获取用户的角色列表
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <returns>角色列表</returns>
    Task<IList<string>> GetRolesAsync(User user);

    /// <summary>
    /// 将用户添加到角色
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="role">角色</param>
    /// <returns>身份验证结果</returns>
    Task<IdentityResult> AddToRoleAsync(User user, Role role);

    /// <summary>
    /// 检查用户是否可以登录
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="password">密码</param>
    /// <param name="lockoutOnFailure">若登录失败，是否调用方法记录一次失败</param>
    /// <returns>登录结果</returns>
    Task<SignInResult> CheckForSignInAsync(User user, string password, bool lockoutOnFailure);

    /// <summary>
    /// 确认手机号
    /// </summary>
    /// <param name="userId">用户Id</param>
    Task ConfirmPhoneNumberAsync(Guid userId);

    /// <summary>
    /// 更新手机号
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <param name="phoneNum">手机号</param>
    Task UpdatePhoneNumberAsync(Guid userId, string phoneNum);

    /// <summary>
    /// 移除用户
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <returns>身份验证结果</returns>
    Task<IdentityResult> RemoveUserAsync(Guid userId);

    /// <summary>
    /// 添加管理员用户
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="phoneNum">手机号</param>
    /// <returns>身份验证结果、用户实体和密码</returns>
    Task<(IdentityResult identityResult, User? user, string? password)> AddAdminUserAsync(string userName, string phoneNum);

    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <returns>身份验证结果、用户实体和密码</returns>
    Task<(IdentityResult identityResult, User? user, string? password)> ResetPasswordAsync(Guid userId);

}

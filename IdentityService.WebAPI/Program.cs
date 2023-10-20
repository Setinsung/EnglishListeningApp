using Commons.CommonInitializer;
using IdentityService.Domain;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure;
using IdentityService.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureDbConfiguration();
builder.ConfigureExtraServices(new InitializerOptions
{
    LogFilePath = "e:/dev_log/IdentityService.log",
    EventBusQueueName = "IdentityService.WebAPI"
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "IdentityService.WebAPI", Version = "v1" });
});
//builder.Services.AddEndpointsApiExplorer();

// Identity配置
builder.Services.AddDataProtection();
builder.Services.AddIdentityCore<User>(opt =>
{
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequiredLength = 6;
    // 不设定RequireUniqueEmail，将不允许邮箱为空
    // 以下用于将GenerateEmailConfirmationTokenAsync验证码缩短
    opt.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
    opt.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
});
IdentityBuilder identityBuilder = new(typeof(User), typeof(Role), builder.Services);
identityBuilder
    .AddEntityFrameworkStores<IdDbContext>()
    .AddDefaultTokenProviders() 
    .AddRoleManager<RoleManager<Role>>()
    .AddUserManager<UserManager<User>>();


//if (builder.Environment.IsDevelopment())
builder.Services.AddScoped<IEmailSender, MockEmailSender>();
builder.Services.AddScoped<ISmsSender, MockSmsSender>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityService.WebAPI v1"));
}

app.UseCommonDefault();

app.MapControllers();

app.Run();

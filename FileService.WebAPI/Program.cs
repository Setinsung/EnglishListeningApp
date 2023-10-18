using Commons.CommonInitializer;
using FileService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureDbConfiguration();
builder.ConfigureExtraServices(new InitializerOptions
{
    LogFilePath = "e:/dev_log/FileService.log",
    EventBusQueueName = "FileService.WebAPI"
});

builder.Services.AddOptions()
    .Configure<SMBStorageOptions>(builder.Configuration.GetSection("FileService:SMB"));

builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    o => o.SwaggerDoc("v1", new() { Title = "FileService.WebAPI", Version = "v1" })
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileService.WebAPI v1"));
}

app.UseStaticFiles();

app.UseCommonDefault();

app.MapControllers();

app.Run();

using Commons.CommonInitializer;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureDbConfiguration();
builder.ConfigureExtraServices(new InitializerOptions
{
    LogFilePath = "e:/dev_log/Listening.Admin.log",
    EventBusQueueName = "Listening.WebAPI.Admin"
});
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Listening.WebAPI.Admin", Version = "v1" });
});
//builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSignalR();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Listening.WebAPI.Admin v1"));
}

app.UseCommonDefault();

app.MapControllers();

app.Run();

using MiniGuard.API.Hubs;
using MiniGuard.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS — allow Angular dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// SignalR
builder.Services.AddSignalR();

// Log Simulator — singleton so it can be injected as ILogSimulator AND run as IHostedService
builder.Services.AddSingleton<LogSimulator>();
builder.Services.AddSingleton<ILogSimulator>(sp => sp.GetRequiredService<LogSimulator>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<LogSimulator>());

// AnomalyService — added in Phase 3
// builder.Services.AddSingleton<IAnomalyService, AnomalyService>();

var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// CORS before routing
app.UseCors("AllowAngular");

app.UseAuthorization();
app.MapControllers();

// SignalR hub
app.MapHub<MonitorHub>("/monitorhub");

app.Run();

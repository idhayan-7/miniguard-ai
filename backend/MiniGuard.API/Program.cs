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

// Application services (implementations added in Phase 2 & 3)
// builder.Services.AddSingleton<ILogSimulator, LogSimulator>();
// builder.Services.AddHostedService(sp => (LogSimulator)sp.GetRequiredService<ILogSimulator>());
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

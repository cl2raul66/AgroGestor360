using AgroGestor360.Server.Sevices;
using AgroGestor360.Server.Tools.Enums;
using AgroGestor360.Server.Tools.Hubs;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IClientdeviceForLitedbService, ClientdeviceForLitedbService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();

builder.Services.AddControllers();

var app = builder.Build();

var hubContext = app.Services.GetService<IHubContext<NotificationHub>>();
if (hubContext is not null)
{
    Console.WriteLine("El servidor está iniciado");
    NotificationHub.Status = ServerStatus.Running;
    await hubContext.Clients.All.SendAsync("ReceiveStatusMessage", NotificationHub.Status.ToString());
}

// Configure the HTTP request pipeline.

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapHub<NotificationHub>("/serverStatusHub");
    _ = endpoints.MapHealthChecks("/healthchecks", new HealthCheckOptions
    {
        ResultStatusCodes =
        {
            [HealthStatus.Healthy] = StatusCodes.Status200OK,
            [HealthStatus.Degraded] = StatusCodes.Status200OK,
            [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
        }
    });
});

app.MapControllers();

var applicationLifetime = app.Services.GetService<IHostApplicationLifetime>();
applicationLifetime?.ApplicationStopping.Register(async () =>
{
    if (hubContext is not null)
    {
        Console.WriteLine("El servidor se va a detener");
        NotificationHub.Status = ServerStatus.Running;
        await hubContext.Clients.All.SendAsync("ReceiveStatusMessage", NotificationHub.Status.ToString());
    }
});

app.Run();

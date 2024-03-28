using AgroGestor360.Server.Sevices;
using AgroGestor360.Server.Tools.Hubs;
using AgroGestor360.Server.Tools.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IShareholderForLitedbService, ShareholderForLitedbService>();
builder.Services.AddSingleton<ISellerForLitedbService, SellerForLitedbService>();
builder.Services.AddSingleton<ICustomerForLitedbService, CustomerForLitedbService>();

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseRouting();

// Add the middle-ware here
app.UseMiddleware<DeviceTypeRestrictionMiddleware>();

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

app.Run();

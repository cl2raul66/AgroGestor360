using AgroGestor360.Server.Services;
using AgroGestor360.Server.Tools.Configurations;
using AgroGestor360.Server.Tools.Enums;
using AgroGestor360.Server.Tools.Hubs;
using AgroGestor360.Server.Tools.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<BanksDbConfig>();
builder.Services.AddSingleton<ContactsDbConfig>();
builder.Services.AddSingleton<MerchandiseDbConfig>();
builder.Services.AddSingleton<ProductsDbConfig>();
builder.Services.AddSingleton<IBankAccountsLiteDbService,BankAccountsLiteDbService>();
builder.Services.AddSingleton<IShareholderForLitedbService, ShareholderForLitedbService>();
builder.Services.AddSingleton<IMerchandiseInLiteDbService, MerchandiseInLiteDbService>();
builder.Services.AddSingleton<IArticlesForWarehouseInLiteDbService, ArticlesForWarehouseInLiteDbService>();
builder.Services.AddSingleton<IArticlesForSalesInLiteDbService, ArticlesForSalesInLiteDbService>();
builder.Services.AddSingleton<ICustomersInLiteDbService, CustomersInLiteDbService>();
builder.Services.AddSingleton<ISellersInLiteDbService, SellersInLiteDbService>();
builder.Services.AddSingleton<IProductsForSalesInLiteDbService, ProductsForSalesInLiteDbService>();
builder.Services.AddSingleton<IQuotesInLiteDbService, QuotesInLiteDbService>();
builder.Services.AddSingleton<IWasteQuotationInLiteDbService, WasteQuotationInLiteDbService>();
builder.Services.AddSingleton<IOrdersInLiteDbService, OrdersInLiteDbService>();
builder.Services.AddSingleton<IWasteOrdersInLiteDbService, WasteOrdersInLiteDbService>();
builder.Services.AddSingleton<IInvoicesInLiteDbService, InvoicesInLiteDbService>();
builder.Services.AddSingleton<IWasteInvoicesInLiteDbService, WasteInvoicesInLiteDbService>();

builder.Services.AddHostedService<PeriodicTaskService>();
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();

var app = builder.Build();

var hubContext = app.Services.GetService<IHubContext<NotificationHub>>();
if (hubContext is not null)
{
    NotificationHub.Status = ServerStatus.Running;
    await hubContext.Clients.All.SendAsync("ReceiveStatusMessage", NotificationHub.Status);
}

//// Get the IHostApplicationLifetime service
//var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

//// Get the IHubContext service
//var hubContext = app.Services.GetRequiredService<IHubContext<NotificationHub>>();

//// Register the ApplicationStarted event
//lifetime.ApplicationStarted.Register(() =>
//{
//    // The application has started, send a status notification
//    hubContext.Clients.All.SendAsync("ReceiveStatusMessage", ServerStatus.Running);
//});

//// Register the ApplicationStopping event
//lifetime.ApplicationStopping.Register(() =>
//{
//    // The application is stopping, send a status notification
//    hubContext.Clients.All.SendAsync("ReceiveStatusMessage", ServerStatus.Stopped);
//});

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

var applicationLifetime = app.Services.GetService<IHostApplicationLifetime>();
applicationLifetime?.ApplicationStopping.Register(async () =>
{
    if (hubContext is not null)
    {
        NotificationHub.Status = ServerStatus.Stopped;
        await hubContext.Clients.All.SendAsync("ReceiveStatusMessage", NotificationHub.Status);
    }
});

app.Run();

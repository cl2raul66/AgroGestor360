using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Enums;
using AgroGestor360.Server.Tools.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AgroGestor360.Server.Services;

public class PeriodicTaskService : IHostedService, IDisposable
{
    Timer? _timer;
    readonly IHubContext<NotificationHub> _hubContext;
    readonly IConfiguration configurationServ;
    readonly IQuotesInLiteDbService quotesServ;
    readonly IWasteQuotationInLiteDbService wasteQuotationServ;
    readonly IOrdersInLiteDbService ordersServ;
    readonly IWasteOrdersInLiteDbService wasteOrdersServ;

    public PeriodicTaskService(IHubContext<NotificationHub> hubContext, IConfiguration configurationService, IQuotesInLiteDbService quotesService, IWasteQuotationInLiteDbService wasteQuotationService, IOrdersInLiteDbService ordersService, IWasteOrdersInLiteDbService wasteOrdersService)
    {
        _hubContext = hubContext;
        configurationServ = configurationService;
        quotesServ = quotesService;
        wasteQuotationServ = wasteQuotationService;
        ordersServ = ordersService;
        wasteOrdersServ = wasteOrdersService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }

    void DoWork(object? state)
    {
        CheckDateExpirableInQuotation();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    void CheckDateExpirableInQuotation()
    {
        var expiryDays = configurationServ.GetValue<int>("QuotationSettings:ExpiryDays");
        var quotations = quotesServ.GetExpiringQuotes(expiryDays);

        var cancelledQuotations = new List<Quotation>();

        foreach (var quotation in quotations)
        {
            Quotation cancelledQuotation = new()
            {
                Date = quotation.Date,
                Code = quotation.Code,
                Seller = quotation.Seller,
                Customer = quotation.Customer,
                Products = quotation.Products,
                Status = QuotationStatus.Cancelled
            };

            cancelledQuotations.Add(cancelledQuotation);
        }

        var codesToDelete = quotations.Select(x => x.Code);

        quotesServ.BeginTrans();
        wasteQuotationServ.BeginTrans();
        
        try
        {
            var results = wasteQuotationServ.InsertMany(cancelledQuotations);

            if (results)
            {
                var deleteResults = quotesServ.DeleteMany(codesToDelete);

                if (deleteResults)
                {
                    _hubContext.Clients.Group("QuotationView").SendAsync("ReceiveExpiredQuotationMessage", codesToDelete.ToArray());
                }
                else
                {
                    quotesServ.Rollback();
                    wasteQuotationServ.Rollback();
                }
            }

            quotesServ.Commit();
            wasteQuotationServ.Commit();
        }
        catch
        {
            quotesServ.Rollback();
            wasteQuotationServ.Rollback();
            throw;
        }
    }

    void CheckDateExpirableInOrder()
    {
        var expiryDays = configurationServ.GetValue<int>("OrderSettings:ExpiryDays");
        var orders = ordersServ.GetExpiringOrders(expiryDays);

        var cancelledOrders = new List<Order>();

        foreach (var order in orders)
        {
            Order cancelledOrder = new()
            {
                Date = order.Date,
                Code = order.Code,
                Seller = order.Seller,
                Customer = order.Customer,
                Products = order.Products,
                Status = OrderStatus.Cancelled
            };

            cancelledOrders.Add(cancelledOrder);
        }

        var codesToDelete = orders.Select(x => x.Code);

        ordersServ.BeginTrans();
        wasteOrdersServ.BeginTrans();

        try
        {
            var results = wasteOrdersServ.InsertMany(cancelledOrders);

            if (results)
            {
                var deleteResults = ordersServ.DeleteMany(codesToDelete);

                if (deleteResults)
                {
                    _hubContext.Clients.Group("OrderView").SendAsync("ReceiveExpiredOrderMessage", codesToDelete.ToArray());
                }
                else
                {
                    ordersServ.Rollback();
                    wasteOrdersServ.Rollback();
                }
            }

            ordersServ.Commit();
            wasteOrdersServ.Commit();
        }
        catch
        {
            ordersServ.Rollback();
            wasteOrdersServ.Rollback();
            throw;
        }
    }

}

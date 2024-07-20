using AgroGestor360.Server.Models;
using AgroGestor360.Server.Services;
using AgroGestor360.Server.Tools;
using AgroGestor360.Server.Tools.Enums;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using UnitsNet;
using static AgroGestor360.Server.Models.SaleReport;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportsController : ControllerBase
{
    readonly IConfiguration config;
    readonly IQuotesInLiteDbService quotesServ;
    readonly ISaleRecordsInLiteDbService saleRecordsServ;
    readonly IWasteSaleRecordsInLiteDbService wasteSaleRecordsServ;

    public ReportsController(IConfiguration configuration, IQuotesInLiteDbService quotesService, ISaleRecordsInLiteDbService saleRecordsService, IWasteSaleRecordsInLiteDbService wasteSaleRecordsService)
    {
        config = configuration;
        quotesServ = quotesService;
        saleRecordsServ = saleRecordsService;
        wasteSaleRecordsServ = wasteSaleRecordsService;
    }

    [HttpGet("customerquotereport/{code}")]
    public ActionResult<CustomerQuoteReport> GetCustomerQuoteReport(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest();
        }

        try
        {
            var organization = config.GetSection("Organization").Get<Organization>();
            var quotation = quotesServ.GetByCode(code);

            List<CustomerQuoteReport.ProductTable> products = [];
            foreach (var p in quotation.Products!)
            {
                CustomerQuoteReport.ProductTable productItem = new()
                {
                    ProductName = $"{p.Product!.ProductName} {p.Product!.Packaging!.Value:F2} {p.Product!.Packaging!.Unit}",
                    ProductQuantity = p.Quantity,
                    ArticlePrice = p.Product!.ArticlePrice
                };

                if (p.HasCustomerDiscount)
                {
                    productItem.ProductName = $"{p.Product!.ProductName} {p.Product!.Packaging!.Value:F2} {p.Product!.Packaging!.Unit} (Descuento)";
                    productItem.ArticlePrice -= productItem.ArticlePrice * (quotation.Customer!.Discount!.Discount / 100);
                }

                if (p.OfferId > 0)
                {
                    var o = p.Product!.Offering![p.OfferId - 1];
                    productItem.ProductName = $"{p.Product!.ProductName}-{p.OfferId} {p.Product!.Packaging!.Value:F2} {p.Product!.Packaging!.Unit} (Oferta {o.BonusAmount} {(o.BonusAmount == 1 ? "unidad" : "unidades")} extra)";
                    productItem.ProductQuantity = o.Quantity;
                }

                products.Add(productItem);
            }

            CustomerQuoteReport report = new()
            {
                OrganizationName = organization?.Name,
                OrganizationAddress = organization?.Address,
                OrganizationPhone = organization?.Phone,
                OrganizationEmail = organization?.Email,

                QuotationCode = code,
                QuotationDate = quotation.Date,

                SellerName = quotation.Seller!.Contact!.FormattedName,

                CustomerName = string.IsNullOrEmpty(quotation.Customer?.Contact?.Organization?.Name)
                    ? quotation.Customer?.Contact?.FormattedName
                    : quotation.Customer?.Contact?.Organization?.Name,
                CustomerPhone = quotation.Customer!.Contact!.PhoneNumbers?.FirstOrDefault().Number,

                ProductItems = [.. products],
                TotalAmount = products.Sum(p => p.ProductQuantity * p.ArticlePrice)
            };

            return Ok(report);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpPost("SaleReport")]
    public ActionResult<SaleReport> GetSaleReport(SaleReportParameters dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        try
        {
            SaleStatus? statusToFilter = null;

            var sellerId = string.IsNullOrEmpty(dTO.SellerId) ? null : new ObjectId(dTO.SellerId);
            var customerId = string.IsNullOrEmpty(dTO.CustomerId) ? null : new ObjectId(dTO.CustomerId);

            List<SaleTable> saleItems = [];
            IEnumerable<WasteSaleRecord> getWasteSaleRecords = [];
            IEnumerable<SaleRecord> getSaleRecords = [];

            switch (dTO.ReportState)
            {
                case "Pagadas":
                    statusToFilter = SaleStatus.Paid;
                    getWasteSaleRecords = wasteSaleRecordsServ.GetWasteSaleRecordsByDateRange(dTO.EndDate, dTO.BeginDate, statusToFilter, sellerId, customerId);

                    if (getWasteSaleRecords is null || !getWasteSaleRecords.Any())
                    {
                        return NotFound();
                    }
                    saleItems.AddRange(CreateSaleTablesFromWasteSaleRecords(getWasteSaleRecords!, dTO));
                    break;
                case "Pendientes":
                    statusToFilter = SaleStatus.Pending;
                    getSaleRecords = saleRecordsServ.GetSaleRecordsByDateRange(dTO.EndDate, dTO.BeginDate, statusToFilter, sellerId, customerId);

                    if (getSaleRecords is null || !getSaleRecords.Any())
                    {
                        return NotFound();
                    }
                    saleItems.AddRange(CreateSaleTablesFromSaleRecords(getSaleRecords!, dTO));
                    break;
                case "Canceladas":
                    statusToFilter = SaleStatus.Cancelled;
                    getWasteSaleRecords = wasteSaleRecordsServ.GetWasteSaleRecordsByDateRange(dTO.EndDate, dTO.BeginDate, statusToFilter, sellerId, customerId);
                    if (getWasteSaleRecords is null || !getWasteSaleRecords.Any())
                    {
                        return NotFound();
                    }
                    saleItems.AddRange(CreateSaleTablesFromWasteSaleRecords(getWasteSaleRecords!, dTO));
                    break;
                default:
                    getSaleRecords = saleRecordsServ.GetSaleRecordsByDateRange(dTO.EndDate, dTO.BeginDate, statusToFilter, sellerId, customerId);
                    getWasteSaleRecords = wasteSaleRecordsServ.GetWasteSaleRecordsByDateRange(dTO.EndDate, dTO.BeginDate, statusToFilter, sellerId, customerId);

                    if (getSaleRecords is not null && getSaleRecords!.Any())
                    {
                        saleItems.AddRange(CreateSaleTablesFromSaleRecords(getSaleRecords!, dTO));
                    }
                    if (getWasteSaleRecords is not null && getSaleRecords!.Any())
                    {
                        saleItems.AddRange(CreateSaleTablesFromWasteSaleRecords(getWasteSaleRecords!, dTO));
                    }

                    if ((getSaleRecords is null || !getSaleRecords.Any()) && (getWasteSaleRecords is null || !getWasteSaleRecords.Any()))
                    {
                        return NotFound();
                    }
                    break;
            }

            var organization = config.GetSection("Organization").Get<Organization>();

            SaleReport report = new()
            {
                IssueDate = DateTime.Now,
                OrderBy = dTO.OrderBy,
                OrganizationName = organization!.Name,
                OrganizationPhone = organization!.Phone,
                OrganizationEmail = organization!.Email,
                OrganizationAddress = organization!.Address,
                SaleState = dTO.ReportState,
                SaleItems = [.. saleItems]
            };

            return Ok(report);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    #region EXTRA
    IEnumerable<SaleTable> CreateSaleTablesFromWasteSaleRecords(IEnumerable<WasteSaleRecord> invoices, SaleReportParameters dTO)
    {
        foreach (var item in invoices)
        {
            double totalToPay = GetTotalAmount.Get(item);
            double totalPaid = item.Status switch
            {
                SaleStatus.Paid => totalToPay,
                _ => item.PaymentMethods is null || item.PaymentMethods.Length == 0 ? item.PaymentMethods?.Sum(x => x.Amount) ?? 0 : item.PaymentMethods!.Sum(x => x.Amount)
            };

            DateTime? invoiceDate = null;
            if (item.Status is SaleStatus.Paid)
            {
                if (item.PaymentMethods is not null && item.PaymentMethods.Length != 0)
                {
                    invoiceDate = item.PaymentMethods.Last().Date;
                }
                if (item.PaymentMethods is not null && item.PaymentMethods.Length != 0)
                {
                    invoiceDate = item.PaymentMethods.Last().Date;
                }
            }

            yield return new SaleTable
            {
                Code = item.Code,
                Seller = item.Seller?.Contact!.FormattedName,
                Customer = string.IsNullOrEmpty(item.Customer?.Contact!.Organization?.Name)
                    ? item.Customer?.Contact!.FormattedName
                    : item.Customer?.Contact!.Organization?.Name,
                SaleEntryDate = item.Date,
                SaleDate = invoiceDate,
                SaleStatus = item.Status,
                TotalToPay = totalToPay,
                TotalPaid = totalPaid
            };
        }
    }

    IEnumerable<SaleTable> CreateSaleTablesFromSaleRecords(IEnumerable<SaleRecord> invoices, SaleReportParameters dTO)
    {
        foreach (var item in invoices)
        {
            double totalToPay = GetTotalAmount.Get(item);
            double totalPaid = item.Status switch
            {
                SaleStatus.Paid => totalToPay,
                _ => item.PaymentMethods is null || item.PaymentMethods.Length == 0 ? item.PaymentMethods?.Sum(x => x.Amount) ?? 0 : item.PaymentMethods!.Sum(x => x.Amount)
            };

            DateTime? invoiceDate = null;
            if (item.PaymentMethods is not null && item.PaymentMethods.Length != 0)
            {
                invoiceDate = item.PaymentMethods.Last().Date;
            }

            yield return new SaleTable
            {
                Code = item.Code,
                Seller = item.Seller?.Contact!.FormattedName,
                Customer = string.IsNullOrEmpty(item.Customer?.Contact!.Organization?.Name)
                    ? item.Customer?.Contact!.FormattedName
                    : item.Customer?.Contact!.Organization?.Name,
                SaleEntryDate = item.Date,
                SaleDate = invoiceDate,
                SaleStatus = item.Status,
                TotalToPay = totalToPay,
                TotalPaid = totalPaid
            };
        }
    }
    #endregion
}

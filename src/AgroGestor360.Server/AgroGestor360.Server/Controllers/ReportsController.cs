using AgroGestor360.Server.Models;
using AgroGestor360.Server.Services;
using Microsoft.AspNetCore.Mvc;
using UnitsNet;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportsController : ControllerBase
{
    readonly IConfiguration config;
    readonly IQuotesInLiteDbService quotesServ;

    public ReportsController(IConfiguration configuration, IQuotesInLiteDbService quotesService)
    {
        config = configuration;
        quotesServ = quotesService;
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

            List<CustomerQuoteReport.Product> products = [];
            foreach (var p in quotation.Products!)
            {
                CustomerQuoteReport.Product productItem = new()
                {
                    ProductName = $"{p.Product!.ProductName} {p.Product!.Packaging!.Value:F2} {p.Product!.Packaging!.Unit}",
                    ProductQuantity = p.Quantity,
                    ArticlePrice = p.Product!.ArticlePrice
                };

                if (p.HasCustomerDiscount)
                {
                    productItem.ProductName = $"{p.Product!.ProductName} {p.Product!.Packaging!.Value:F2} {p.Product!.Packaging!.Unit} (Descuento)";
                    productItem.ArticlePrice -= productItem.ArticlePrice * (quotation.Customer!.Discount!.Value / 100);
                }

                if (p.OfferId > 0)
                {
                    var o = p.Product!.Offering![p.OfferId-1];
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

}

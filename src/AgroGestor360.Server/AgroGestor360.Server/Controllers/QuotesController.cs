using AgroGestor360.Server.Models;
using AgroGestor360.Server.Services;
using AgroGestor360.Server.Tools;
using AgroGestor360.Server.Tools.Enums;
using AgroGestor360.Server.Tools.Extensions;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class QuotesController : ControllerBase
{
    readonly IQuotesInLiteDbService quotesServ;
    readonly IWasteQuotationInLiteDbService wasteQuotationServ;
    readonly ICustomersInLiteDbService customersServ;
    readonly ISellersInLiteDbService sellersServ;
    readonly IProductsForSalesInLiteDbService productsForSalesServ;
    readonly IArticlesForWarehouseInLiteDbService articlesForWarehouseServ;

    public QuotesController(IQuotesInLiteDbService quotesService, IWasteQuotationInLiteDbService wasteQuotationService, ICustomersInLiteDbService customersService, ISellersInLiteDbService sellersService, IProductsForSalesInLiteDbService productsForSalesService, IArticlesForWarehouseInLiteDbService articlesForWarehouseService)
    {
        quotesServ = quotesService;
        wasteQuotationServ = wasteQuotationService;
        customersServ = customersService;
        sellersServ = sellersService;
        productsForSalesServ = productsForSalesService;
        articlesForWarehouseServ = articlesForWarehouseService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = quotesServ.Exist;

        return Ok(exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<DTO7>> GetAll()
    {
        var all = quotesServ.GetAll()?.Select(x => x.ToDTO7()) ?? [];

        return all is null || !all.Any() ? NotFound() : Ok(all);
    }

    [HttpGet("{code}")]
    public ActionResult<DTO7> GetByCode(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest();
        }

        var found = quotesServ.GetByCode(code);
        if (found is null)
        {
            return NotFound();
        }

        return Ok(found.ToDTO7());
    }

    [HttpGet("getproductsbycode/{code}")]
    public ActionResult<DTO7_4> GetProductsByCode(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest();
        }

        var found = quotesServ.GetByCode(code);
        if (found is null)
        {
            return NotFound();
        }

        double totalAmount = GetTotalAmount.Get(found);

        DTO7_4 dTO = new()
        {
            Date = found.Date,
            Code = found.Code,
            TotalAmount = totalAmount,
            CustomerName = found.Customer?.Contact?.FormattedName,
            SellerName = found.Seller?.Contact?.FormattedName,
            Status = found.Status,
            Products = found.Products?.Select(p => ProductItemForDocumentToString.GetText(p.Quantity, p.Product!, p.HasCustomerDiscount, p.OfferId, found.Customer!)).ToArray()
        };

        return Ok(dTO);
    }

    [HttpGet("getorderbycode/{code}")]
    public ActionResult<DTO8_2> GetOrderByCode(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest();
        }

        var found = quotesServ.GetByCode(code);
        if (found is null)
        {
            return NotFound();
        }

        DTO8_2 dTO = new()
        {
            Code = found.Code,
            CustomerId = found.Customer!.Id!.ToString(),
            SellerId = found.Seller!.Id!.ToString(),
            ProductItems = found.Products!.Select(x => new DTO9()
            {
                HasCustomerDiscount = x.HasCustomerDiscount,
                OfferId = x.OfferId,
                ProductItemForSaleId = x.Product!.Id!.ToString(),
                Quantity = x.Quantity
            }).ToArray(),
        };

        return Ok(dTO);
    }

    [HttpPost]
    public ActionResult<string> Insert(DTO7_1 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        var customer = customersServ.GetById(new ObjectId(dTO.CustomerId));
        var seller = sellersServ.GetById(new ObjectId(dTO.SellerId));
        List<ProductSaleBase> productItems = [];

        foreach (var item in dTO.ProductItems!)
        {
            ProductItemForSale product = productsForSalesServ.GetById(new ObjectId(item.ProductItemForSaleId));
            ProductSaleBase productItemForQuotation = new()
            {
                HasCustomerDiscount = item.HasCustomerDiscount,
                OfferId = item.OfferId,
                Quantity = item.Quantity,
                Product = product
            };

            productItems.Add(productItemForQuotation);
        }

        Quotation entity = new()
        {
            Status = dTO.Status,
            Code = ShortGuidHelper.Generate(),
            Date = dTO.Date,
            Seller = seller,
            Customer = customer,
            Products = [.. productItems]
        };
        var result = quotesServ.Insert(entity);

        return string.IsNullOrEmpty(result) ? NotFound() : Ok(result);
    }

    [HttpPut]
    public IActionResult Update([FromBody] DTO7_2 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        Quotation entity = quotesServ.GetByCode(dTO.Code!);
        if (entity is null)
        {
            return NotFound();
        }

        var customerId = new ObjectId(dTO.CustomerId);
        var sellerId = new ObjectId(dTO.SellerId);

        if (!customersServ.ExistById(customerId))
        {
            var customer = customersServ.GetById(customerId);
            entity.Customer = customer;
        }

        if (!sellersServ.ExistById(sellerId))
        {
            var seller = sellersServ.GetById(sellerId);
            entity.Seller = seller;
        }

        List<ProductSaleBase> productItems = [];

        foreach (var item in dTO.ProductItems!)
        {
            ProductItemForSale product = productsForSalesServ.GetById(new ObjectId(item.ProductItemForSaleId));
            ProductSaleBase productItemForQuotation = new()
            {
                HasCustomerDiscount = item.HasCustomerDiscount,
                OfferId = item.OfferId,
                Quantity = item.Quantity,
                Product = product
            };

            productItems.Add(productItemForQuotation);
        }

        entity.Products = [.. productItems];

        var result = quotesServ.Update(entity);

        return !result ? NotFound() : Ok();
    }

    [HttpPut("changerbystatus")]
    public IActionResult ChangesByStatus(DTO7_3 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        Quotation entity = quotesServ.GetByCode(dTO.Code!);
        if (entity is null)
        {
            return NotFound();
        }

        switch (dTO.Status)
        {
            case QuotationStatus.Draft:
            case QuotationStatus.Sent:
                entity.Status = dTO.Status;
                var resultUpdate = quotesServ.Update(entity);
                return !resultUpdate ? NotFound() : Ok();
            case QuotationStatus.Accepted:
            case QuotationStatus.Rejected:
                quotesServ.BeginTrans();
                var wasDelete = quotesServ.Delete(entity.Code!);
                if (!wasDelete)
                {
                    quotesServ.Rollback();
                    return NotFound();
                }

                entity.Status = dTO.Status;
                wasteQuotationServ.BeginTrans();
                var resultInsert = wasteQuotationServ.Insert(entity);
                if (string.IsNullOrEmpty(resultInsert))
                {
                    quotesServ.Rollback();
                    wasteQuotationServ.Rollback();
                    return NotFound();
                }
                quotesServ.Commit();
                wasteQuotationServ.Commit();
                return Ok();
            case QuotationStatus.Cancelled:
                quotesServ.BeginTrans();
                var resultDelete = quotesServ.Delete(entity.Code!);
                if (!resultDelete)
                {
                    quotesServ.Rollback();
                    return NotFound();
                }

                entity.Status = dTO.Status;
                wasteQuotationServ.BeginTrans();
                var resultInsert1 = wasteQuotationServ.Insert(entity);
                if (string.IsNullOrEmpty(resultInsert1))
                {
                    quotesServ.Rollback();
                    wasteQuotationServ.Rollback();
                    return NotFound();
                }
                quotesServ.Commit();
                wasteQuotationServ.Commit();
                return Ok();
            default:
                return BadRequest();
        }
    }

    [HttpDelete("{code}")]
    public IActionResult Delete(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest();
        }

        var CurrentQuotation = quotesServ.GetByCode(code);
        if (CurrentQuotation is null)
        {
            return NotFound();
        }

        var resultDelete = quotesServ.Delete(code);
        if (!resultDelete)
        {
            return NotFound();
        }

        return Ok();
    }

    #region EXTRA
    
    #endregion
}

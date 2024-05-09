using AgroGestor360.Server.Models;
using AgroGestor360.Server.Services;
using AgroGestor360.Server.Tools.Extensions;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using UnitsNet;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class QuotesController : ControllerBase
{
    readonly IQuotesInLiteDbService quotesServ;
    readonly ICustomersInLiteDbService customersServ;
    readonly ISellersInLiteDbService sellersServ;
    readonly IProductsForSalesInLiteDbService productsForSalesServ;

    public QuotesController(IQuotesInLiteDbService quotesService, ICustomersInLiteDbService customersService, ISellersInLiteDbService sellersService, IProductsForSalesInLiteDbService productsForSalesService)
    {
        quotesServ = quotesService;
        customersServ = customersService;
        sellersServ = sellersService;
        productsForSalesServ = productsForSalesService;
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

    [HttpGet("{id}")]
    public ActionResult<DTO7> GetById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        var found = quotesServ.GetById(new Guid(id));
        if (found is null)
        {
            return NotFound();
        }

        return Ok(found.ToDTO7());
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
        List<ProductItemForQuotation> productItems = [];

        foreach (var item in dTO.ProductItems!)
        {
            ProductItemForSale product = productsForSalesServ.GetById(new ObjectId(item.ProductId));
            ProductItemForQuotation productItemForQuotation = new()
            {
                HasCustomerDiscount = item.HasCustomerDiscount,
                OfferId = item.OfferId,
                Quantity = item.Quantity,
                Product = product,

            };

            productItems.Add(productItemForQuotation);
        }

        Quotation entity = new()
        {
            WasDelivered = false,
            Code = Guid.NewGuid(),
            QuotationDate = dTO.QuotationDate,
            Seller = seller,
            Customer = customer,
            ProductItems = [.. productItems]
        };
        var result = quotesServ.Insert(entity);

        return string.IsNullOrEmpty(result) ? NotFound() : Ok(result);
    }

    [HttpPut]
    public IActionResult UpdateQuotationWithNormalPrice([FromBody] DTO7_2 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        var customer = customersServ.GetById(new ObjectId(dTO.CustomerId));
        var seller = sellersServ.GetById(new ObjectId(dTO.SellerId));
        List<ProductItemForQuotation> productItems = [];

        foreach (var item in dTO.ProductItems!)
        {
            ProductItemForSale product = productsForSalesServ.GetById(new ObjectId(item.ProductId));
            ProductItemForQuotation productItemForQuotation = new()
            {
                HasCustomerDiscount = item.HasCustomerDiscount,
                OfferId = item.OfferId,
                Quantity = item.Quantity,
                Product = product,

            };

            productItems.Add(productItemForQuotation);
        }

        var entity = quotesServ.GetById(new Guid(dTO.Code!));
        entity.Seller = seller;
        entity.Customer = customer;
        entity.ProductItems = [.. productItems];

        var result = quotesServ.Update(entity);

        return !result ? NotFound() : Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        var result = quotesServ.Delete(new Guid(id));

        return !result ? NotFound() : Ok();
    }
}

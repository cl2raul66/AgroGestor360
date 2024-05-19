using AgroGestor360.Server.Models;
using AgroGestor360.Server.Services;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class InvoicesController : ControllerBase
{
    readonly IInvoicesInLiteDbService invoicesServ;
    readonly ISellersInLiteDbService sellersServ;
    readonly ICustomersInLiteDbService customersServ;
    readonly IProductsForSalesInLiteDbService productsForSalesServ;

    public InvoicesController(IInvoicesInLiteDbService invoicesService, ISellersInLiteDbService sellersService, ICustomersInLiteDbService customersService, IProductsForSalesInLiteDbService productsForSalesService)
    {
        invoicesServ = invoicesService;
        sellersServ = sellersService;
        customersServ = customersService;
        productsForSalesServ = productsForSalesService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = invoicesServ.Exist;

        return Ok(exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<DTO10>> GetAll()
    {
        var all = invoicesServ.GetAll().Select(CreateDTO10);

        return all is not null && all.Any() ? Ok(all) : NotFound();
    }

    [HttpGet("{code}")]
    public ActionResult<DTO10> GetByCode(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest();
        }

        var found = invoicesServ.GetByCode(Guid.Parse(code));
        if (found is null)
        {
            return NotFound();
        }

        var entity = CreateDTO10(found);

        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public ActionResult<string> Insert(DTO10_1 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        var customer = customersServ.GetById(new ObjectId(dTO.CustomerId));
        var seller = sellersServ.GetById(new ObjectId(dTO.SellerId));

        var productsIds = dTO.Products?.Select(x => new ObjectId(x.ProductItemForSaleId)) ?? [];
        var products = productsForSalesServ.GetManyById(productsIds);
        if (products is not null || products!.Any())
        {
            List<ProductSaleBase> productItems = [];
            foreach (var ele in products!)
            {
                var dTO9 = dTO.Products!.First(x => x.ProductItemForSaleId == ele.Id!.ToString());
                var productSaleBase = CreateProductSaleBase(dTO9, ele);
                productItems.Add(productSaleBase);
            }


            Invoice entity = new()
            {
                Code = Guid.NewGuid(),
                Date = dTO.Date,
                Seller = seller,
                Customer = customer,
                Products = [.. productItems],
                NumberFEL = dTO.NumberFEL,
                ImmediatePayments = dTO.ImmediatePayments,
                CreditsPayments = dTO.CreditsPayments,
                Status = dTO.Status
            };

            var result = invoicesServ.Insert(entity);

            return string.IsNullOrEmpty(result) ? NotFound() : Ok(result);
        }

        return NotFound();
    }

    [HttpPut("UpdateByPaymentMethod")]
    public IActionResult UpdateByPaymentMethod(DTO10_2 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        var found = invoicesServ.GetByCode(Guid.Parse(dTO.Code!));
        if (found is null)
        {
            return NotFound();
        }

        if (dTO.ImmediateMethod is not null)
        {
            List<ImmediatePayment> immediatePayments = new(found.ImmediatePayments ?? [])
            {
                dTO.ImmediateMethod
            };
            found.ImmediatePayments = [.. immediatePayments];
        }

        if (dTO.CreditPaymentMethod is not null)
        {
            List<CreditPayment> creditPayments = new(found.CreditsPayments ?? [])
            {
                dTO.CreditPaymentMethod
            };
            found.CreditsPayments = [.. creditPayments];
        }

        var result = invoicesServ.Update(found);

        return result ? Ok() : NotFound();
    }

    [HttpDelete("{code}")]
    public IActionResult Delete(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest();
        }

        var result = invoicesServ.Delete(new Guid(code));

        return !result ? NotFound() : Ok();
    }

    #region EXTRA
    DTO10 CreateDTO10(Invoice entity)
    {
        double totalAmount = 0;
        foreach (var pi in entity.Products!)
        {
            double itemPrice = pi.Product!.ArticlePrice;
            if (pi.HasCustomerDiscount)
            {
                double discount = entity.Customer!.Discount!.Value;
                itemPrice -= itemPrice * (discount / 100);
            }

            double itemQuantity = pi.Quantity;
            if (pi.OfferId > 0)
            {
                itemQuantity = pi.Product.Offering![pi.OfferId - 1].Quantity;
            }
            totalAmount += itemQuantity * itemPrice;
        }

        return new DTO10
        {
            Code = entity.Code.ToString(),
            Date = entity.Date,
            SellerId = entity.Seller?.Id?.ToString(),
            SellerName = entity.Seller?.Contact?.FormattedName,
            CustomerId = entity.Customer?.Id?.ToString(),
            CustomerName = entity.Customer?.Contact?.FormattedName,
            TotalAmount = totalAmount,
            Paid = entity.CreditsPayments?.Sum(p => p.Amount) ?? 0,
            DaysRemaining = (entity.Date - DateTime.Now).Days,
            NumberFEL = entity.NumberFEL,
            Status = entity.Status
        };
    }

    ProductSaleBase CreateProductSaleBase(DTO9 dTO, ProductItemForSale product)
    {
        return new ProductSaleBase
        {
            HasCustomerDiscount = dTO.HasCustomerDiscount,
            OfferId = dTO.OfferId,
            Quantity = dTO.Quantity,
            Product = product
        };
    }
    #endregion
}

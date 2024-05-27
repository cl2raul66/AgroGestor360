﻿using AgroGestor360.Server.Models;
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
public class OrdersController : ControllerBase
{
    readonly IOrdersInLiteDbService ordersServ;
    readonly ICustomersInLiteDbService customersServ;
    readonly ISellersInLiteDbService sellersServ;
    readonly IProductsForSalesInLiteDbService productsForSalesServ;
    readonly IQuotesInLiteDbService quotesServ;
    readonly IArticlesForWarehouseInLiteDbService articlesForWarehouseServ;

    public OrdersController(IOrdersInLiteDbService ordersService, ICustomersInLiteDbService customersService, ISellersInLiteDbService sellersService, IProductsForSalesInLiteDbService productsForSalesService, IQuotesInLiteDbService quotesService, IArticlesForWarehouseInLiteDbService articlesForWarehouseService)
    {
        ordersServ = ordersService;
        customersServ = customersService;
        sellersServ = sellersService;
        productsForSalesServ = productsForSalesService;
        quotesServ = quotesService;
        articlesForWarehouseServ = articlesForWarehouseService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = ordersServ.Exist;

        return Ok(exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<DTO8>> GetAll()
    {
        var all = ordersServ.GetAll();
        if (all is null || !all.Any())
        {
            return NotFound();
        }
        OrderStatus orderStatus = OrderStatus.Processing;
        var quantityByArticle = articlesForWarehouseServ.GetAll().Select(x => (x.MerchandiseId, x.Quantity));
        List<DTO8> result = [];
        foreach (var item in all)
        {
            if (orderStatus is not OrderStatus.Pending)
            {
                foreach (var pi in item.Products!)
                {
                    var stock = quantityByArticle.FirstOrDefault(x => x.MerchandiseId == pi.Product!.MerchandiseId).Quantity;
                    if (pi.OfferId > 0)
                    {
                        var isMayor = pi.Product!.Offering![pi.OfferId - 1].Quantity > stock;
                        if (isMayor)
                        {
                            orderStatus = OrderStatus.Pending;
                        }
                    }
                    if (pi.Quantity > stock)
                    {
                        orderStatus = OrderStatus.Pending;
                    }
                }
            }

            result.Add(item.ToDTO8());
        }

        return Ok(result);
    }

    [HttpGet("{code}")]
    public ActionResult<DTO8> GetByCode(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest();
        }

        var found = ordersServ.GetByCode(code);
        if (found is null)
        {
            return NotFound();
        }

        UpdateOrderStatusBasedOnStock(found);

        return Ok(found.ToDTO8());
    }

    [HttpGet("getproductsbycode/{code}")]
    public ActionResult<DTO8_5> GetProductsByCode(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest();
        }

        var found = ordersServ.GetByCode(code);
        if (found is null)
        {
            return NotFound();
        }

        double totalAmount = GetTotalAmount.Get(found);

        DTO8_5 dTO = new()
        {
            Date = found.Date,
            Code = found.Code,
            TotalAmount = totalAmount,
            CustomerName = found.Customer?.Contact?.FormattedName,
            SellerName = found.Seller?.Contact?.FormattedName,
            Status = found.Status,
            Products = found.Products?.Select(p => ProductItemForDocumentToString.GetText(p.Product!, p.HasCustomerDiscount, p.OfferId, found.Customer!)).ToArray()
        };

        return Ok(dTO);
    }

    [HttpPost("getdto8_4fromquotation")]
    public ActionResult<DTO8_4> GetDTO8_4FromQuotation(DTO7 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        var customerId = new ObjectId(dTO.CustomerId);
        var customer = customersServ.GetById(customerId);
        var dto5_1 = customer.ToDTO5_1();

        var sellerId = new ObjectId(dTO.SellerId);
        var seller = sellersServ.GetById(sellerId);
        var dto6 = seller.ToDTO6();

        DTO8_4 result = new()
        {
            Code = dTO.Code,
            Customer = dto5_1,
            Seller = dto6
        };

        var quotation = quotesServ.GetByCode(new(dTO.Code!));
        var products = new List<DTO9>();
        foreach (var productItem in quotation.Products!)
        {
            var product = productItem.Product;
            DTO9 dTO9 = new()
            {
                ProductItemForSaleId = product!.Id!.ToString(),
                Quantity = productItem.Quantity,
                OfferId = productItem.OfferId,
                HasCustomerDiscount = productItem.HasCustomerDiscount
            };
            products.Add(dTO9);
        }

        result.Products = [.. products];

        return Ok(result);
    }

    [HttpPost]
    public ActionResult<string> Insert(DTO8_1 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        var customer = customersServ.GetById(new ObjectId(dTO.CustomerId));
        var seller = sellersServ.GetById(new ObjectId(dTO.SellerId));
        List<ProductSaleBase> productItems = [];
        List<ArticleItemForWarehouse> articleItems = [];

        foreach (var item in dTO.ProductItems!)
        {            
            ProductItemForSale product = productsForSalesServ.GetById(new ObjectId(item.ProductItemForSaleId));
            ProductSaleBase productItemForOrder = new()
            {
                HasCustomerDiscount = item.HasCustomerDiscount,
                OfferId = item.OfferId,
                Quantity = item.Quantity,
                Product = product
            };

            ArticleItemForWarehouse articleItemForWarehouse = articlesForWarehouseServ.GetById(product.MerchandiseId!);
            articleItemForWarehouse.Reserved = item.Quantity;
            articleItemForWarehouse.Quantity -= item.Quantity;

            productItems.Add(productItemForOrder);
            articleItems.Add(articleItemForWarehouse);
        }

        Order entity = new()
        {
            Status = dTO.Status,
            Code = string.IsNullOrEmpty(dTO.Code) ? ShortGuidHelper.Generate() : dTO.Code,
            Date = dTO.Date,
            Seller = seller,
            Customer = customer,
            Products = [.. productItems]
        };

        var resultInsert = ordersServ.Insert(entity);
        if (string.IsNullOrEmpty(resultInsert))
        {
            return NotFound();
        }
        var resultUpdateManyInWarehouse = articlesForWarehouseServ.UpdateMany([.. articleItems]);

        return resultUpdateManyInWarehouse ? Ok(resultInsert) : NotFound();
    }

    [HttpPost("insertfromquote")]
    public ActionResult<string> InsertFromQuote(DTO7 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        var found = quotesServ.GetByCode(dTO.Code!);
        if (found is null)
        {
            return NotFound();
        }

        List<ProductSaleBase> productItems = [];
        List<ArticleItemForWarehouse> articleItems = [];
        bool isPending = false;

        foreach (var item in found.Products!)
        {
            ProductItemForSale product = productsForSalesServ.GetById(new ObjectId(item.Product!.Id));
            ProductSaleBase productItemForOrder = new()
            {
                HasCustomerDiscount = item.HasCustomerDiscount,
                OfferId = item.OfferId,
                Quantity = item.Quantity,
                Product = product
            };

            ArticleItemForWarehouse articleItemForWarehouse = articlesForWarehouseServ.GetById(product.MerchandiseId!);
            articleItemForWarehouse.Reserved = item.Quantity;
            articleItemForWarehouse.Quantity -= item.Quantity;

            productItems.Add(productItemForOrder);
            articleItems.Add(articleItemForWarehouse);

            if (!isPending)
            {
                isPending = articleItemForWarehouse.Reserved > articleItemForWarehouse.Quantity;
            }
        }

        Order entity = new()
        {
            Status = isPending ? OrderStatus.Pending : OrderStatus.Processing,
            Code = dTO.Code!,
            Date = DateTime.Now,
            Seller = found.Seller,
            Customer = found.Customer,
            Products = [.. productItems]
        };

        UpdateOrderStatusBasedOnStock(entity);

        var resultInsert = ordersServ.Insert(entity);
        if (string.IsNullOrEmpty(resultInsert))
        {
            return NotFound();
        }
        var resultUpdateManyInWarehouse = articlesForWarehouseServ.UpdateMany([.. articleItems]);

        return resultUpdateManyInWarehouse ? Ok(resultInsert) : NotFound();
    }

    [HttpPut]
    public IActionResult Update([FromBody] DTO8_2 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        Order entity = ordersServ.GetByCode(dTO.Code!);
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
        List<ArticleItemForWarehouse> articleItems = [];

        foreach (var item in dTO.ProductItems!)
        {
            ProductItemForSale product = productsForSalesServ.GetById(new ObjectId(item.ProductItemForSaleId));
            ProductSaleBase productItemForOrder = new()
            {
                HasCustomerDiscount = item.HasCustomerDiscount,
                OfferId = item.OfferId,
                Quantity = item.Quantity,
                Product = product
            };

            ArticleItemForWarehouse articleItemForWarehouse = articlesForWarehouseServ.GetById(product.MerchandiseId!);
            articleItemForWarehouse.Reserved = item.Quantity;
            articleItemForWarehouse.Quantity -= item.Quantity;

            productItems.Add(productItemForOrder);
            articleItems.Add(articleItemForWarehouse);
        }

        entity.Products = [.. productItems];

        var resultUpdate = ordersServ.Update(entity);

        if (!resultUpdate)
        {
            return NotFound();
        }
        var resultUpdateManyInWarehouse = articlesForWarehouseServ.UpdateMany([.. articleItems]);

        return resultUpdateManyInWarehouse ? Ok() : NotFound();
    }

    [HttpPut("updatebyproductsandstatus")]
    public IActionResult UpdateByProductsAndStatus(DTO8_3 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        var entity = ordersServ.GetByCode(dTO.Code!);
        if (entity is null)
        {
            return NotFound();
        }

        List<ProductSaleBase> productItems = [];
        List<ArticleItemForWarehouse> articleItems = [];

        foreach (var item in dTO.ProductItems!)
        {
            ProductItemForSale product = productsForSalesServ.GetById(new ObjectId(item.ProductItemForSaleId));
            ProductSaleBase productItemForOrder = new()
            {
                HasCustomerDiscount = item.HasCustomerDiscount,
                OfferId = item.OfferId,
                Quantity = item.Quantity,
                Product = product
            };

            ArticleItemForWarehouse articleItemForWarehouse = articlesForWarehouseServ.GetById(product.MerchandiseId!);
            articleItemForWarehouse.Reserved = item.Quantity;
            articleItemForWarehouse.Quantity -= item.Quantity;

            productItems.Add(productItemForOrder);
            articleItems.Add(articleItemForWarehouse);
        }

        entity.Products = [.. productItems];
        entity.Status = dTO.Status;

        var resultUpdate = ordersServ.Update(entity);

        if (!resultUpdate)
        {
            return NotFound();
        }
        var resultUpdateManyInWarehouse = articlesForWarehouseServ.UpdateMany([.. articleItems]);

        return resultUpdateManyInWarehouse ? Ok() : NotFound();
    }

    [HttpDelete("{code}")]
    public IActionResult Delete(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest();
        }

        var result = ordersServ.Delete(code);

        return !result ? NotFound() : Ok();
    }

    #region Extra
    private void UpdateOrderStatusBasedOnStock(Order order)
    {
        var quantityByArticle = articlesForWarehouseServ.GetAll().Select(x => (x.MerchandiseId, x.Quantity));

        foreach (var pi in order.Products!)
        {
            if (order.Status is not OrderStatus.Pending)
            {
                var stock = quantityByArticle.FirstOrDefault(x => x.MerchandiseId == pi.Product!.MerchandiseId).Quantity;
                if (pi.OfferId > 0)
                {
                    var isMayor = pi.Product!.Offering![pi.OfferId - 1].Quantity > stock;
                    if (isMayor)
                    {
                        order.Status = OrderStatus.Pending;
                    }
                }
                if (pi.Quantity > stock)
                {
                    order.Status = OrderStatus.Pending;
                }
            }
        }
    }
    #endregion
}

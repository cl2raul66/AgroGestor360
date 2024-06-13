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
public class OrdersController : ControllerBase
{
    readonly IOrdersInLiteDbService ordersServ;
    readonly ICustomersInLiteDbService customersServ;
    readonly ISellersInLiteDbService sellersServ;
    readonly IProductsForSalesInLiteDbService productsForSalesServ;
    readonly IQuotesInLiteDbService quotesServ;
    readonly IArticlesForWarehouseInLiteDbService articlesForWarehouseServ;
    readonly IWasteOrdersInLiteDbService wasteOrdersServ;

    public OrdersController(IOrdersInLiteDbService ordersService, ICustomersInLiteDbService customersService, ISellersInLiteDbService sellersService, IProductsForSalesInLiteDbService productsForSalesService, IQuotesInLiteDbService quotesService, IArticlesForWarehouseInLiteDbService articlesForWarehouseService, IWasteOrdersInLiteDbService wasteOrdersService)
    {
        ordersServ = ordersService;
        customersServ = customersService;
        sellersServ = sellersService;
        productsForSalesServ = productsForSalesService;
        quotesServ = quotesService;
        articlesForWarehouseServ = articlesForWarehouseService;
        wasteOrdersServ = wasteOrdersService;
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

        return Ok(all.Select(x => x.ToDTO8()));
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

        //UpdateOrderStatusBasedOnStock(ref found);

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
            OrganizationName = found.Customer?.Contact?.Organization?.Name,
            CustomerName = found.Customer?.Contact?.FormattedName,
            SellerName = found.Seller?.Contact?.FormattedName,
            Status = found.Status,
            Products = found.Products?.Select(p => ProductItemForDocumentToString.GetText(p.Quantity, p.Product!, p.HasCustomerDiscount, p.OfferId, found.Customer!)).ToArray()
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

        var (productItems, articleItems) = ProcessProductItems(dTO.ProductItems!);

        bool isPending = false;

        foreach (var item in articleItems)
        {
            if (!isPending)
            {
                isPending = item.Reserved > item.Quantity;
            }
        }

        Order entity = new()
        {
            Status = isPending ? OrderStatus.Pending : OrderStatus.Processing,
            Code = string.IsNullOrEmpty(dTO.Code) ? ShortGuidHelper.Generate() : dTO.Code,
            Date = DateTime.Now,
            Seller = seller,
            Customer = customer,
            Products = [.. productItems]
        };

        ordersServ.BeginTrans();
        var resultInsert = ordersServ.Insert(entity);
        if (string.IsNullOrEmpty(resultInsert))
        {
            ordersServ.Rollback();
            return NotFound();
        }

        var warehouseUpdated = UpdateWarehouseAfterInsert(entity);
        if (!warehouseUpdated)
        {
            ordersServ.Rollback();
            return NotFound();
        }

        ordersServ.Commit();
        return Ok(resultInsert);
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

        var productItemsDTO = found.Products!.Select(p => new DTO9
        {
            ProductItemForSaleId = p.Product!.Id!.ToString(),
            Quantity = p.Quantity,
            OfferId = p.OfferId,
            HasCustomerDiscount = p.HasCustomerDiscount
        });

        var (productItems, articleItems) = ProcessProductItems(productItemsDTO);

        bool isPending = false;

        foreach (var item in articleItems)
        {
            if (!isPending)
            {
                isPending = item.Reserved > item.Quantity;
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

        //UpdateOrderStatusBasedOnStock(ref entity);

        ordersServ.BeginTrans();
        var resultInsert = ordersServ.Insert(entity);
        if (string.IsNullOrEmpty(resultInsert))
        {
            ordersServ.Rollback();
            return NotFound();
        }

        var warehouseUpdated = UpdateWarehouseAfterInsert(entity);
        if (!warehouseUpdated)
        {
            ordersServ.Rollback();
            return NotFound();
        }

        ordersServ.Commit();
        return Ok(resultInsert);
    }

    [HttpPut]
    public IActionResult Update(DTO8_2 dTO)
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

        var (productItems, articleItems) = ProcessProductItems(dTO.ProductItems!);

        entity.Products = [.. productItems];

        var resultUpdate = ordersServ.Update(entity);

        if (!resultUpdate)
        {
            return NotFound();
        }
        var resultUpdateManyInWarehouse = articlesForWarehouseServ.UpdateMany(articleItems);

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

        var (productItems, articleItems) = ProcessProductItems(dTO.ProductItems!);

        entity.Products = [.. productItems];
        entity.Status = dTO.Status;

        var resultUpdate = ordersServ.Update(entity);

        if (!resultUpdate)
        {
            return NotFound();
        }
        var resultUpdateManyInWarehouse = articlesForWarehouseServ.UpdateMany(articleItems);

        return resultUpdateManyInWarehouse ? Ok() : NotFound();
    }

    [HttpPut("changebystatus")]
    public IActionResult ChangeByStatus(DTO8_6 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        var found = ordersServ.GetByCode(dTO.Code!);
        if (found is null)
        {
            return NotFound();
        }

        found.Status = dTO.Status;

        if (dTO.Status is OrderStatus.Completed)
        {
            try
            {
                wasteOrdersServ.BeginTrans();
                var wasteInsertResult = wasteOrdersServ.Insert(found);
                if (string.IsNullOrEmpty(wasteInsertResult))
                {
                    wasteOrdersServ.Rollback();
                    return NotFound();
                }

                ordersServ.BeginTrans();
                var deleteResult = ordersServ.Delete(found.Code!);
                if (!deleteResult)
                {
                    wasteOrdersServ.Rollback();
                    ordersServ.Rollback();
                    return NotFound();
                }

                wasteOrdersServ.Commit();
                ordersServ.Commit();
            }
            catch
            {
                wasteOrdersServ.Rollback();
                ordersServ.Rollback();
                return NotFound();
            }

            return Ok();
        }

        if (dTO.Status is OrderStatus.Rejected || dTO.Status is OrderStatus.Cancelled)
        {
            try
            {
                wasteOrdersServ.BeginTrans();
                var wasteInsertResult = wasteOrdersServ.Insert(found);
                if (string.IsNullOrEmpty(wasteInsertResult))
                {
                    wasteOrdersServ.Rollback();
                    return NotFound();
                }

                ordersServ.BeginTrans();
                var deleteResult = ordersServ.Delete(found.Code!);
                if (!deleteResult)
                {
                    wasteOrdersServ.Rollback();
                    ordersServ.Rollback();
                    return NotFound();
                }

                var warehouseUpdated = UpdateWarehouseAfterDeletion(found);
                if (!warehouseUpdated)
                {
                    wasteOrdersServ.Rollback();
                    ordersServ.Rollback();
                    return NotFound();
                }

                wasteOrdersServ.Commit();
                ordersServ.Commit();
            }
            catch
            {
                wasteOrdersServ.Rollback();
                ordersServ.Rollback();
                return NotFound();
            }

            return Ok();
        }

        var resultUpdate = ordersServ.Update(found);

        if (!resultUpdate)
        {
            return NotFound();
        }

        return Ok();
    }

    [HttpDelete("{code}")]
    public IActionResult Delete(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest();
        }

        var CurrentOrder = ordersServ.GetByCode(code);
        if (CurrentOrder is null)
        {
            return NotFound();
        }

        ordersServ.BeginTrans();

        var resultDelete = ordersServ.Delete(code);
        if (!resultDelete)
        {
            ordersServ.Rollback();
            return NotFound();
        }

        var warehouseUpdated = UpdateWarehouseAfterDeletion(CurrentOrder);
        if (!warehouseUpdated)
        {
            ordersServ.Rollback();
            return NotFound();
        }

        ordersServ.Commit();

        return Ok();
    }

    #region Extra
    (List<ProductSaleBase>, List<ArticleItemForWarehouse>) ProcessProductItems(IEnumerable<DTO9> productItemsDTO)
    {
        List<ProductSaleBase> productItems = [];
        List<ArticleItemForWarehouse> articleItems = [];

        var productIds = productItemsDTO.Select(item => new ObjectId(item.ProductItemForSaleId)).ToList();

        var products = productsForSalesServ.GetManyById(productIds).ToDictionary(item => item.Id!, item => item);

        var merchandiseIds = products.Values.Select(product => product.MerchandiseId!).ToList();

        var warehouseItems = articlesForWarehouseServ.GetManyByIds(merchandiseIds).ToDictionary(item => item.MerchandiseId!, item => item);

        foreach (var item in productItemsDTO)
        {
            ProductItemForSale product = products[new ObjectId(item.ProductItemForSaleId)];
            ArticleItemForWarehouse articleItemForWarehouse = warehouseItems[product.MerchandiseId!];

            ProductSaleBase productItemForOrder = new()
            {
                HasCustomerDiscount = item.HasCustomerDiscount,
                OfferId = item.OfferId,
                Quantity = item.Quantity,
                Product = product
            };

            articleItemForWarehouse.Reserved += item.Quantity;
            articleItemForWarehouse.Quantity -= item.Quantity;

            productItems.Add(productItemForOrder);
            articleItems.Add(articleItemForWarehouse);
        }

        return (productItems, articleItems);
    }

    void UpdateOrderStatusBasedOnStock(ref Order order)
    {
        var merchandiseIds = order.Products!.Select(x => x.Product!.MerchandiseId!);
        var quantityByArticle = articlesForWarehouseServ.GetManyByIds(merchandiseIds).Select(x => (x.MerchandiseId, x.Quantity));

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

    bool UpdateWarehouseAfterInsert(SaleBase entity)
    {
        try
        {
            var merchandiseQuantities = entity.Products!.ToDictionary(p => p.Product!.MerchandiseId!, p => FindQuantity(p.Quantity, p.Product!, p.OfferId));

            var warehouseItems = articlesForWarehouseServ.GetManyByIds(merchandiseQuantities.Keys);
            List<ArticleItemForWarehouse> saveArticleItemForWarehouses = [];

            foreach (var item in warehouseItems)
            {
                item.Quantity -= merchandiseQuantities[item.MerchandiseId!];
                item.Reserved += merchandiseQuantities[item.MerchandiseId!];

                saveArticleItemForWarehouses.Add(item);
            }

            return articlesForWarehouseServ.UpdateMany(saveArticleItemForWarehouses);
        }
        catch
        {
            return false;
        }
    }

    bool UpdateWarehouseAfterDeletion(SaleBase entity)
    {
        try
        {
            var merchandiseQuantities = entity.Products!.ToDictionary(p => p.Product!.MerchandiseId!, p => FindQuantity(p.Quantity, p.Product!, p.OfferId));

            var warehouseItems = articlesForWarehouseServ.GetManyByIds(merchandiseQuantities.Keys);
            List<ArticleItemForWarehouse> saveArticleItemForWarehouses = [];

            foreach (var item in warehouseItems)
            {
                item.Quantity += merchandiseQuantities[item.MerchandiseId!];
                item.Reserved -= merchandiseQuantities[item.MerchandiseId!];

                saveArticleItemForWarehouses.Add(item);
            }

            return articlesForWarehouseServ.UpdateMany(saveArticleItemForWarehouses);
        }
        catch
        {
            return false;
        }
    }

    double FindQuantity(double productQuantity, ProductItemForSale product, int offerId)
    {
        double quantity = productQuantity;
        if (offerId > 0)
        {
            var o = product.Offering![offerId - 1];
            quantity = o.Quantity + o.BonusAmount;
        }
        return quantity;
    }
    #endregion
}

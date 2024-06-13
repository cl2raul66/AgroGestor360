﻿using AgroGestor360.Server.Models;
using AgroGestor360.Server.Services;
using AgroGestor360.Server.Tools;
using AgroGestor360.Server.Tools.Enums;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using UnitsNet;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class InvoicesController : ControllerBase
{
    readonly IInvoicesInLiteDbService invoicesServ;
    readonly ISellersInLiteDbService sellersServ;
    readonly ICustomersInLiteDbService customersServ;
    readonly IProductsForSalesInLiteDbService productsForSalesServ;
    readonly IWasteInvoicesInLiteDbService wasteInvoicesServ;
    readonly IArticlesForWarehouseInLiteDbService articlesForWarehouseServ;
    readonly IQuotesInLiteDbService quotesServ;
    readonly IOrdersInLiteDbService ordersServ;
    readonly IConfiguration configurationServ;

    public InvoicesController(IInvoicesInLiteDbService invoicesService, ISellersInLiteDbService sellersService, ICustomersInLiteDbService customersService, IProductsForSalesInLiteDbService productsForSalesService, IWasteInvoicesInLiteDbService wasteInvoicesService, IArticlesForWarehouseInLiteDbService articlesForWarehouseService, IQuotesInLiteDbService quotesService, IOrdersInLiteDbService ordersService, IConfiguration configuration)
    {
        invoicesServ = invoicesService;
        sellersServ = sellersService;
        customersServ = customersService;
        productsForSalesServ = productsForSalesService;
        wasteInvoicesServ = wasteInvoicesService;
        articlesForWarehouseServ = articlesForWarehouseService;
        quotesServ = quotesService;
        ordersServ = ordersService;
        configurationServ = configuration;
    }

    [HttpGet("getcredittime")]
    public ActionResult<IEnumerable<int>> GetCreditTime()
    {
        var days = configurationServ.GetSection("CreditTime:Days").Get<int[]>();

        return days is null ? NotFound() : Ok(days);
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

        var found = invoicesServ.GetByCode(code);
        if (found is null)
        {
            return NotFound();
        }

        var entity = CreateDTO10(found);

        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpGet("getproductsbycode/{code}")]
    public ActionResult<DTO10_4> GetProductsByCode(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest();
        }

        var found = invoicesServ.GetByCode(code);
        if (found is null)
        {
            return NotFound();
        }

        double totalAmount = GetTotalAmount.Get(found);

        DTO10_4 dTO = new()
        {
            Date = found.Date,
            Code = found.Code,
            TotalAmount = totalAmount,
            Paid = found.Status switch
            {
                InvoiceStatus.Paid => totalAmount,
                _ => found.ImmediatePayments is null || found.ImmediatePayments.Length == 0 ? found.CreditsPayments?.Sum(x => x.Amount) ?? 0 : found.ImmediatePayments!.Sum(x => x.Amount)
            },
            DaysRemaining = DaysRemaining(found.Customer!.Credit!.TimeLimit, found.Date),
            OrganizationName = found.Customer?.Contact?.Organization?.Name,
            CustomerName = found.Customer?.Contact?.FormattedName,
            SellerName = found.Seller?.Contact?.FormattedName,
            NumberFEL = found.NumberFEL,
            Status = found.Status,
            Products = found.Products?.Select(p => ProductItemForDocumentToString.GetText(p.Quantity, p.Product!, p.HasCustomerDiscount, p.OfferId, found.Customer!)).ToArray(),
            ImmediatePayments = found.ImmediatePayments,
            CreditsPayments = found.CreditsPayments
        };

        return Ok(dTO);
    }

    [HttpPost]
    public ActionResult<string> Insert(DTO10_1 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        var seller = sellersServ.GetById(new ObjectId(dTO.SellerId));
        var customer = customersServ.GetById(new ObjectId(dTO.CustomerId));
        customer.Credit!.TimeLimit = 0;
        if (dTO.TimeCredit is not null)
        {
            customer.Credit!.TimeLimit = dTO.TimeCredit.TimeLimit;
        }

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
                Code = ShortGuidHelper.Generate(),
                Date = dTO.Date,
                Seller = seller,
                Customer = customer,
                CreditsPayments = [],
                Products = [.. productItems],
                Status = dTO.Status
            };

            invoicesServ.BeginTrans();
            var resultInsert = invoicesServ.Insert(entity);

            if (string.IsNullOrEmpty(resultInsert))
            {
                invoicesServ.Rollback();
                return NotFound();
            }

            var warehouseUpdated = UpdateWarehouseAfterInsert(entity);
            if (!warehouseUpdated)
            {
                invoicesServ.Rollback();
                return NotFound();
            }

            invoicesServ.Commit();
            return Ok(resultInsert);
        }

        return NotFound();
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
                break;
            }
        }

        Invoice entity = new()
        {
            Status = InvoiceStatus.Pending,
            Code = dTO.Code!,
            Date = DateTime.Now,
            Seller = found.Seller,
            Customer = found.Customer,
            Products = [.. productItems]
        };

        invoicesServ.BeginTrans();
        var resultInsert = invoicesServ.Insert(entity);
        if (string.IsNullOrEmpty(resultInsert))
        {
            invoicesServ.Rollback();
            return NotFound();
        }

        var warehouseUpdated = UpdateWarehouseAfterInsert(entity);
        if (!warehouseUpdated)
        {
            invoicesServ.Rollback();
            return NotFound();
        }

        invoicesServ.Commit();
        return Ok(resultInsert);
    }

    [HttpPost("insertfromorder")]
    public ActionResult<string> InsertFromOrder(DTO8 dTO)
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
                break;
            }
        }

        Invoice entity = new()
        {
            Status = InvoiceStatus.Pending,
            Code = dTO.Code!,
            Date = DateTime.Now,
            Seller = found.Seller,
            Customer = found.Customer,
            Products = [.. productItems]
        };

        invoicesServ.BeginTrans();
        var resultInsert = invoicesServ.Insert(entity);
        if (string.IsNullOrEmpty(resultInsert))
        {
            invoicesServ.Rollback();
            return NotFound();
        }

        //var warehouseUpdated = UpdateWarehouseAfterInsert(entity);
        //if (!warehouseUpdated)
        //{
        //    invoicesServ.Rollback();
        //    return NotFound();
        //}

        invoicesServ.Commit();
        return Ok(resultInsert);
    }

    [HttpPut("depreciationupdate")]
    public IActionResult DepreciationUpdate(DTO10_2 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        var found = invoicesServ.GetByCode(dTO.Code!);
        if (found is null)
        {
            return NotFound();
        }

        double totalAmount = GetTotalAmount.Get(found);

        if (dTO.ImmediateMethod is not null)
        {
            List<ImmediatePayment> immediatePayments = new(found.ImmediatePayments ?? [])
            {
                dTO.ImmediateMethod
            };

            found.ImmediatePayments = [.. immediatePayments];

            if (found.ImmediatePayments.Sum(x => x.Amount) == totalAmount)
            {
                found.Status = InvoiceStatus.Paid;
                WasteInvoice wasteInvoice = WasteInvoiceFabric(found);
                var resultInsert = wasteInvoicesServ.Insert(wasteInvoice);
                if (string.IsNullOrEmpty(resultInsert) || resultInsert != dTO.Code)
                {
                    return NotFound();
                }
                var resultDelete = invoicesServ.Delete(dTO.Code!);
                return resultDelete ? Ok() : NotFound();
            }
        }

        if (dTO.CreditPaymentMethod is not null)
        {
            List<CreditPayment> creditPayments = new(found.CreditsPayments ?? [])
            {
                dTO.CreditPaymentMethod
            };

            found.CreditsPayments = [.. creditPayments];

            if (found.CreditsPayments.Sum(x => x.Amount) == totalAmount)
            {
                found.Status = InvoiceStatus.Paid;
                WasteInvoice wasteInvoice = WasteInvoiceFabric(found);
                var resultInsert = wasteInvoicesServ.Insert(wasteInvoice);
                if (string.IsNullOrEmpty(resultInsert) || resultInsert != dTO.Code)
                {
                    return NotFound();
                }
                var resultDelete = invoicesServ.Delete(dTO.Code!);
                return resultDelete ? Ok() : NotFound();
            }
        }

        var result = invoicesServ.Update(found);

        return result ? Ok() : NotFound();
    }

    [HttpPut("changebystatus")]
    public IActionResult ChangeByStatus(DTO10_3 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        var found = invoicesServ.GetByCode(dTO.Code!);
        if (found is null)
        {
            return NotFound();
        }

        found.Status = dTO.Status;

        if (dTO.Status is InvoiceStatus.Paid)
        {
            wasteInvoicesServ.BeginTrans();
            WasteInvoice wasteInvoice = WasteInvoiceFabric(found);
            var resultInsert = wasteInvoicesServ.Insert(wasteInvoice);
            if (string.IsNullOrEmpty(resultInsert) || resultInsert != dTO.Code)
            {
                wasteInvoicesServ.Rollback();
                return NotFound();
            }

            invoicesServ.BeginTrans();
            var resultDelete = invoicesServ.Delete(dTO.Code!);
            if (!resultDelete)
            {
                wasteInvoicesServ.Rollback();
                invoicesServ.Rollback();
                return NotFound();
            }

            wasteInvoicesServ.Commit();
            invoicesServ.Commit();
            return Ok();
        }

        if (dTO.Status is InvoiceStatus.Cancelled)
        {
            string concept = string.Empty;
            if (!string.IsNullOrEmpty(dTO.Notes))
            {
                concept = dTO.Notes!.Trim().ToUpper();
                var foundConcept = invoicesServ.GetConceptByNote(concept);
                if (foundConcept is null)
                {
                    var newConcept = new ConceptForDeletedInvoice
                    {
                        Concept = concept
                    };
                    var insertResult = invoicesServ.InsertConcept(newConcept);
                    if (insertResult < 1)
                    {
                        return NotFound();
                    }
                }
            }

            try
            {
                wasteInvoicesServ.BeginTrans();
                WasteInvoice wasteInvoice = WasteInvoiceFabric(found, concept);
                var wasteInsertResult = wasteInvoicesServ.Insert(wasteInvoice);
                if (string.IsNullOrEmpty(wasteInsertResult))
                {
                    wasteInvoicesServ.Rollback();
                    return NotFound();
                }

                invoicesServ.BeginTrans();
                var deleteResult = invoicesServ.Delete(found.Code!);
                if (!deleteResult)
                {
                    wasteInvoicesServ.Rollback();
                    invoicesServ.Rollback();
                    return NotFound();
                }

                var warehouseUpdated = UpdateWarehouseAfterDeletion(found);
                if (!warehouseUpdated)
                {
                    wasteInvoicesServ.Rollback();
                    invoicesServ.Rollback();
                    return NotFound();
                }

                wasteInvoicesServ.Commit();
                invoicesServ.Commit();
            }
            catch
            {
                wasteInvoicesServ.Rollback();
                ordersServ.Rollback();
                return NotFound();
            }

            return Ok();

            //wasteInvoicesServ.BeginTrans();
            //WasteInvoice wasteInvoice = WasteInvoiceFabric(found, concept);
            //var resultInsert = wasteInvoicesServ.Insert(wasteInvoice);
            //if (string.IsNullOrEmpty(resultInsert) || resultInsert != dTO.Code)
            //{
            //    wasteInvoicesServ.Rollback();
            //    return NotFound();
            //}

            //var articlesForWarehouse = articlesForWarehouseServ.GetManyByIds(found.Products!.Select(p => p.Product!.MerchandiseId!).ToArray());

            //var productQuantities = found.Products!.ToDictionary(p => p.Product!.MerchandiseId!, p => p.Quantity);

            //List<ArticleItemForWarehouse> updateArticles = [];

            //foreach (var article in articlesForWarehouse)
            //{
            //    if (productQuantities.TryGetValue(article.MerchandiseId!, out double value))
            //    {
            //        var updateArticle = new ArticleItemForWarehouse
            //        {
            //            Quantity = article.Quantity + value,
            //            Reserved = article.Reserved - value,
            //            MerchandiseName = article.MerchandiseName,
            //            MerchandiseId = article.MerchandiseId,
            //            Packaging = article.Packaging
            //        };

            //        updateArticles.Add(updateArticle);
            //    }
            //}

            //articlesForWarehouseServ.BeginTrans();
            //var updateResult = articlesForWarehouseServ.UpdateMany(updateArticles);
            //if (!updateResult)
            //{
            //    wasteInvoicesServ.Rollback();
            //    articlesForWarehouseServ.Rollback();
            //    return NotFound();
            //}

            //invoicesServ.BeginTrans();
            //var resultDelete = invoicesServ.Delete(dTO.Code!);
            //if (!resultDelete)
            //{
            //    wasteInvoicesServ.Rollback();
            //    invoicesServ.Rollback();
            //    articlesForWarehouseServ.Rollback();
            //    return NotFound();
            //}

            //wasteInvoicesServ.Commit();
            //invoicesServ.Commit();
            //articlesForWarehouseServ.Commit();

            //return Ok();
        }

        return NotFound();
    }

    [HttpDelete("{code}")]
    public IActionResult Delete(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest();
        }

        var found = invoicesServ.GetByCode(code);
        if (found is null)
        {
            return NotFound();
        }

        var articlesForWarehouse = articlesForWarehouseServ.GetManyByIds(found.Products!.Select(p => p.Product!.MerchandiseId!).ToArray());

        var productQuantities = found.Products!.ToDictionary(p => p.Product!.MerchandiseId!, p => p.Quantity);

        List<ArticleItemForWarehouse> updateArticles = [];

        foreach (var article in articlesForWarehouse)
        {
            if (productQuantities.TryGetValue(article.MerchandiseId!, out double value))
            {
                var updateArticle = new ArticleItemForWarehouse
                {
                    Quantity = article.Quantity + value,
                    Reserved = article.Reserved - value,
                    MerchandiseName = article.MerchandiseName,
                    MerchandiseId = article.MerchandiseId,
                    Packaging = article.Packaging
                };

                updateArticles.Add(updateArticle);
            }
        }

        articlesForWarehouseServ.BeginTrans();
        var updateResult = articlesForWarehouseServ.UpdateMany(updateArticles);
        if (!updateResult)
        {
            wasteInvoicesServ.Rollback();
            articlesForWarehouseServ.Rollback();
            return NotFound();
        }

        invoicesServ.BeginTrans();
        var resultDelete = invoicesServ.Delete(code);
        if (!resultDelete)
        {
            wasteInvoicesServ.Rollback();
            invoicesServ.Rollback();
            articlesForWarehouseServ.Rollback();
            return NotFound();
        }

        wasteInvoicesServ.Commit();
        invoicesServ.Commit();
        articlesForWarehouseServ.Commit();

        return Ok();
    }


    [HttpGet("concepts")]
    public ActionResult<IEnumerable<ConceptForDeletedInvoice>> GetConcepts()
    {
        var result = invoicesServ.GetConcepts();

        return result is not null && result.Any() ? Ok(result) : NotFound();
    }

    [HttpPost("concepts")]
    public ActionResult<int> InsertConcept(ConceptForDeletedInvoice entity)
    {
        if (entity is null)
        {
            return BadRequest();
        }

        var result = invoicesServ.InsertConcept(entity);

        return result > 0 ? Ok(result) : NotFound();
    }

    [HttpDelete("concepts/{id}")]
    public IActionResult DeleteConcept(int id)
    {
        if (id < 1)
        {
            return BadRequest();
        }

        var result = invoicesServ.DeleteConcept(id);

        return !result ? NotFound() : Ok();
    }

    #region EXTRA
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

    DTO10 CreateDTO10(Invoice entity)
    {
        double totalAmount = GetTotalAmount.Get(entity);
        double paid = entity.Status switch
        {
            InvoiceStatus.Paid => totalAmount,
            _ => entity.ImmediatePayments is null || entity.ImmediatePayments.Length == 0 ? entity.CreditsPayments?.Sum(x => x.Amount) ?? 0 : entity.ImmediatePayments!.Sum(x => x.Amount)
        };

        var result = new DTO10
        {
            Code = entity.Code,
            Date = entity.Date,
            SellerId = entity.Seller?.Id?.ToString(),
            SellerName = entity.Seller?.Contact?.FormattedName,
            CustomerId = entity.Customer?.Id?.ToString(),
            CustomerName = string.IsNullOrEmpty(entity.Customer?.Contact?.Organization?.Name)
                    ? entity.Customer?.Contact?.FormattedName
                    : entity.Customer?.Contact?.Organization?.Name,
            TotalAmount = totalAmount,
            Paid = paid,
            DaysRemaining = entity.CreditsPayments is null ? 0 : DaysRemaining(entity.Customer!.Credit!.TimeLimit, entity.Date),
            NumberFEL = entity.NumberFEL,
            Status = entity.Status
        };

        return result;
    }

    WasteInvoice WasteInvoiceFabric(Invoice invoice)
    {
        return new WasteInvoice
        {
            Date = invoice.Date,
            Code = invoice.Code,
            Seller = invoice.Seller,
            Customer = invoice.Customer,
            Products = invoice.Products,
            Status = invoice.Status,
            NumberFEL = invoice.NumberFEL,
            ImmediatePayments = invoice.ImmediatePayments,
            CreditsPayments = invoice.CreditsPayments
        };
    }

    WasteInvoice WasteInvoiceFabric(Invoice invoice, string notes)
    {
        var result = WasteInvoiceFabric(invoice);
        result.Notes = notes;
        return result;
    }

    int DaysRemaining(int numberOfInstallments, DateTime date)
    {
        if (numberOfInstallments > 0)
        {
            DateTime endDate = date.AddDays(numberOfInstallments);
            return (endDate - DateTime.Now).Days;
        }
        return 0;
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

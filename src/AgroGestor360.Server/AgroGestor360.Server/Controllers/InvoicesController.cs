using AgroGestor360.Server.Models;
using AgroGestor360.Server.Services;
using AgroGestor360.Server.Tools;
using AgroGestor360.Server.Tools.Enums;
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
    readonly IConfiguration configurationServ;

    public InvoicesController(IInvoicesInLiteDbService invoicesService, ISellersInLiteDbService sellersService, ICustomersInLiteDbService customersService, IProductsForSalesInLiteDbService productsForSalesService, IWasteInvoicesInLiteDbService wasteInvoicesService, IArticlesForWarehouseInLiteDbService articlesForWarehouseService, IConfiguration configuration)
    {
        invoicesServ = invoicesService;
        sellersServ = sellersService;
        customersServ = customersService;
        productsForSalesServ = productsForSalesService;
        wasteInvoicesServ = wasteInvoicesService;
        articlesForWarehouseServ = articlesForWarehouseService;
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

        var found = invoicesServ.GetByCode(Guid.Parse(code));
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

        var found = invoicesServ.GetByCode(new Guid(code));
        if (found is null)
        {
            return NotFound();
        }

        double totalAmount = GetTotalAmount.Get(found);

        DTO10_4 dTO = new()
        {
            Date = found.Date,
            Code = found.Code.ToString(),
            TotalAmount = totalAmount,
            Paid = found.Status switch
            {
                InvoiceStatus.Paid => totalAmount,
                _ => found.ImmediatePayments is null || found.ImmediatePayments.Length == 0 ? found.CreditsPayments?.Sum(x => x.Amount) ?? 0 : found.ImmediatePayments!.Sum(x => x.Amount)
            },
            DaysRemaining = DaysRemaining(found.NumberOfInstallments, found.Date),
            CustomerName = found.Customer?.Contact?.FormattedName,
            SellerName = found.Seller?.Contact?.FormattedName,
            NumberFEL = found.NumberFEL,
            Status = found.Status,
            Products = found.Products?.Select(p => ProductItemForDocumentToString(p.Product!, p.HasCustomerDiscount, p.OfferId, found.Customer!)).ToArray(),
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

            invoicesServ.BeginTrans();

            var result = invoicesServ.Insert(entity);

            if (string.IsNullOrEmpty(result))
            {
                invoicesServ.Rollback();
                return NotFound();
            }

            var articlesForWarehouse = articlesForWarehouseServ.GetManyById(productItems.Select(p => p.Product!.MerchandiseId!).ToArray());

            var productQuantities = productItems.ToDictionary(p => p.Product!.MerchandiseId!, p => p.Quantity);

            List<ArticleItemForWarehouse> updateArticles = [];

            foreach (var article in articlesForWarehouse)
            {
                if (productQuantities.TryGetValue(article.MerchandiseId!, out double value))
                {
                    var updateArticle = new ArticleItemForWarehouse
                    {
                        Quantity = article.Quantity - value,
                        Reserved = value,
                        MerchandiseName = article.MerchandiseName,
                        MerchandiseId = article.MerchandiseId,
                        Packaging = article.Packaging
                    };

                    updateArticles.Add(updateArticle);
                }
            }

            var updateResult = articlesForWarehouseServ.UpdateMany(updateArticles);

            if (!updateResult)
            {
                invoicesServ.Rollback();
                return NotFound();
            }

            invoicesServ.Commit();

            return Ok(result);
        }

        return NotFound();
    }


    //[HttpPost]
    //public ActionResult<string> Insert(DTO10_1 dTO)
    //{
    //    if (dTO is null)
    //    {
    //        return BadRequest();
    //    }

    //    var customer = customersServ.GetById(new ObjectId(dTO.CustomerId));
    //    var seller = sellersServ.GetById(new ObjectId(dTO.SellerId));

    //    var productsIds = dTO.Products?.Select(x => new ObjectId(x.ProductItemForSaleId)) ?? [];
    //    var products = productsForSalesServ.GetManyById(productsIds);
    //    if (products is not null || products!.Any())
    //    {
    //        List<ProductSaleBase> productItems = [];
    //        foreach (var ele in products!)
    //        {
    //            var dTO9 = dTO.Products!.First(x => x.ProductItemForSaleId == ele.Id!.ToString());
    //            var productSaleBase = CreateProductSaleBase(dTO9, ele);
    //            productItems.Add(productSaleBase);
    //        }

    //        Invoice entity = new()
    //        {
    //            Code = Guid.NewGuid(),
    //            Date = dTO.Date,
    //            Seller = seller,
    //            Customer = customer,
    //            Products = [.. productItems],
    //            NumberFEL = dTO.NumberFEL,
    //            ImmediatePayments = dTO.ImmediatePayments,
    //            CreditsPayments = dTO.CreditsPayments,
    //            Status = dTO.Status
    //        };

    //        var result = invoicesServ.Insert(entity);

    //        return string.IsNullOrEmpty(result) ? NotFound() : Ok(result);
    //    }

    //    return NotFound();
    //}

    [HttpPut("depreciationupdate")]
    public IActionResult DepreciationUpdate(DTO10_2 dTO)
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
                var resultInsert = wasteInvoicesServ.Insert(found);
                if (string.IsNullOrEmpty(resultInsert) || resultInsert != dTO.Code)
                {
                    return NotFound();
                }
                var resultDelete = invoicesServ.Delete(Guid.Parse(dTO.Code!));
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
                var resultInsert = wasteInvoicesServ.Insert(found);
                if (string.IsNullOrEmpty(resultInsert) || resultInsert != dTO.Code)
                {
                    return NotFound();
                }
                var resultDelete = invoicesServ.Delete(Guid.Parse(dTO.Code!));
                return resultDelete ? Ok() : NotFound();
            }
        }

        var result = invoicesServ.Update(found);

        return result ? Ok() : NotFound();
    }

    [HttpPut("updatestate")]
    public IActionResult UpdateState(DTO10_3 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        if (dTO.Status is InvoiceStatus.Cancelled)
        {
            var found = invoicesServ.GetByCode(Guid.Parse(dTO.Code!));
            if (found is null)
            {
                return NotFound();
            }

            found.Status = dTO.Status;

            // Iniciar transacción
            wasteInvoicesServ.BeginTrans();
            invoicesServ.BeginTrans();
            articlesForWarehouseServ.BeginTrans();

            var resultInsert = wasteInvoicesServ.Insert(found);
            if (string.IsNullOrEmpty(resultInsert) || resultInsert != dTO.Code)
            {
                // Revertir transacción si la inserción falla
                wasteInvoicesServ.Rollback();
                invoicesServ.Rollback();
                articlesForWarehouseServ.Rollback();
                return NotFound();
            }

            // Restaurar las cantidades de los productos en el almacén
            var articlesForWarehouse = articlesForWarehouseServ.GetManyById(found.Products!.Select(p => p.Product!.MerchandiseId!).ToArray());

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

            var updateResult = articlesForWarehouseServ.UpdateMany(updateArticles);

            if (!updateResult)
            {
                // Revertir transacción si la actualización falla
                wasteInvoicesServ.Rollback();
                invoicesServ.Rollback();
                articlesForWarehouseServ.Rollback();
                return NotFound();
            }

            var resultDelete = invoicesServ.Delete(Guid.Parse(dTO.Code!));
            if (!resultDelete)
            {
                // Revertir transacción si la eliminación falla
                wasteInvoicesServ.Rollback();
                invoicesServ.Rollback();
                articlesForWarehouseServ.Rollback();
                return NotFound();
            }

            // Confirmar transacción si todas las operaciones son exitosas
            wasteInvoicesServ.Commit();
            invoicesServ.Commit();
            articlesForWarehouseServ.Commit();

            return Ok();
        }
        return NotFound();
    }

    //[HttpPut("updatestate")]
    //public IActionResult UpdateState(DTO10_3 dTO)
    //{
    //    if (dTO is null)
    //    {
    //        return BadRequest();
    //    }

    //    if (dTO.Status is InvoiceStatus.Cancelled)
    //    {
    //        var found = invoicesServ.GetByCode(Guid.Parse(dTO.Code!));
    //        if (found is null)
    //        {
    //            return NotFound();
    //        }

    //        found.Status = dTO.Status;

    //        var resultInsert = wasteInvoicesInLiteDbServ.Insert(found);
    //        if (string.IsNullOrEmpty(resultInsert) || resultInsert != dTO.Code)
    //        {
    //            return NotFound();
    //        }

    //        var resultDelete = invoicesServ.Delete(Guid.Parse(dTO.Code!));

    //        return resultDelete ? Ok() : NotFound();
    //    }
    //    return NotFound();
    //}

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
        double totalAmount = GetTotalAmount.Get(entity);
        double paid = entity.Status switch
        {
            InvoiceStatus.Paid => totalAmount,
            _ => entity.ImmediatePayments is null || entity.ImmediatePayments.Length == 0 ? entity.CreditsPayments?.Sum(x => x.Amount) ?? 0 : entity.ImmediatePayments!.Sum(x => x.Amount)
        };

        var result = new DTO10
        {
            Code = entity.Code.ToString(),
            Date = entity.Date,
            SellerId = entity.Seller?.Id?.ToString(),
            SellerName = entity.Seller?.Contact?.FormattedName,
            CustomerId = entity.Customer?.Id?.ToString(),
            CustomerName = entity.Customer?.Contact?.FormattedName,
            TotalAmount = totalAmount,
            Paid = paid,
            DaysRemaining = DaysRemaining(entity.NumberOfInstallments, entity.Date),
            NumberFEL = entity.NumberFEL,
            Status = entity.Status
        };

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

    string ProductItemForDocumentToString(ProductItemForSale product, bool hasCustomerDiscount, int offerId, Customer customer)
    {
        string texto;

        if (hasCustomerDiscount)
        {
            texto = string.Format("{0,0:F2} {1,-20:F2} {2,0:F2} {3,0} (Descuento) {4,-10:N2}",
                product.ProductQuantity,
                product.ProductName,
                product.Packaging!.Value,
                product.Packaging!.Unit,
                product.ArticlePrice -= product.ArticlePrice * (customer!.Discount!.Value / 100.00));
        }
        else if (offerId > 0)
        {
            var o = product.Offering![offerId - 1];
            texto = string.Format("{0,0:F2}-{1,0} {2,0:F2} {3,0} (Oferta {4,-2} y {5,-2} extra) {6,-10:N2}",
                product.ProductQuantity,
                product.ProductName,
                product.Packaging!.Value,
                product.Packaging!.Unit,
                o.BonusAmount,
                o.BonusAmount == 1 ? "unidad" : "unidades",
                product.ArticlePrice);
        }
        else
        {
            texto = string.Format("{0,0:F2} {1,-20:F2} {2,0:F2} {3,0} {4,-10:N2}",
                product.ProductQuantity,
                product.ProductName,
                product.Packaging!.Value,
                product.Packaging!.Unit,
                product.ArticlePrice);
        }

        return texto;
    }
    #endregion
}

using AgroGestor360.Server.Models;
using AgroGestor360.Server.Services;
using AgroGestor360.Server.Tools.Extensions;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class MerchandiseController : ControllerBase
{
    readonly IMerchandiseInLiteDbService merchandiseServ;
    readonly IArticlesForWarehouseInLiteDbService articlesForWarehouseServ;
    readonly IArticlesForSalesInLiteDbService articlesForSalesServ;
    readonly IProductsForSalesInLiteDbService productsForSalesServ;

    public MerchandiseController(IMerchandiseInLiteDbService merchandiseService, IArticlesForWarehouseInLiteDbService articlesForWarehouseService, IArticlesForSalesInLiteDbService articlesForSalesIService, IProductsForSalesInLiteDbService productsForSalesService)
    {
        merchandiseServ = merchandiseService;
        articlesForWarehouseServ = articlesForWarehouseService;
        articlesForSalesServ = articlesForSalesIService;
        productsForSalesServ = productsForSalesService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = merchandiseServ.Exist;
        return Ok(exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<DTO1>> GetAll()
    {
        var all = merchandiseServ.GetAll()?.Select(x => x.ToDTO1()) ?? [];
        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("getallcategories")]
    public ActionResult<IEnumerable<string>> GetAllCategories()
    {
        var all = merchandiseServ.GetAllCategories() ?? [];
        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("{id}")]
    public ActionResult<DTO1> GetById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        var find = merchandiseServ.GetById(new ObjectId(id));
        if (find is null)
        {
            return NotFound();
        }
        var dto = find.ToDTO1();
        return Ok(dto);
    }

    [HttpPost]
    public ActionResult<string> Post([FromBody] DTO1 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        merchandiseServ.BeginTrans();

        var entityMerchandise = dTO.FromDTO1();
        entityMerchandise.Id = ObjectId.NewObjectId();
        var resultMerchandise = merchandiseServ.Insert(entityMerchandise);

        if (string.IsNullOrEmpty(resultMerchandise))
        {
            merchandiseServ.Rollback();
            return NotFound();
        }

        articlesForWarehouseServ.BeginTrans();
        var entityArticleItemForWarehouse = ArticleItemForWarehouseFabric(entityMerchandise);
        var resultArticleItemForWarehouse = articlesForWarehouseServ.Insert(entityArticleItemForWarehouse);

        if (string.IsNullOrEmpty(resultArticleItemForWarehouse))
        {
            merchandiseServ.Rollback();
            articlesForWarehouseServ.Rollback();
            return NotFound();
        }

        articlesForSalesServ.BeginTrans();
        var entityArticleItemForSale = ArticleItemForSaleFabric(entityMerchandise);
        var resultArticleItemForSales = articlesForSalesServ.Insert(entityArticleItemForSale);

        if (string.IsNullOrEmpty(resultArticleItemForSales))
        {
            merchandiseServ.Rollback();
            articlesForWarehouseServ.Rollback();
            articlesForSalesServ.Rollback();
            return NotFound();
        }

        AllCommit();
        return Ok(resultMerchandise);
    }

    [HttpPut]
    public IActionResult Put([FromBody] DTO1 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        var entityMerchandise = dTO.FromDTO1();

        var Quantity = articlesForWarehouseServ.GetById(entityMerchandise.Id!)?.Quantity ?? 0;
        var Price = articlesForSalesServ.GetById(entityMerchandise.Id!)?.Price ?? 0;

        merchandiseServ.BeginTrans();

        var resultMerchandise = merchandiseServ.Update(entityMerchandise);

        if (!resultMerchandise)
        {
            merchandiseServ.Rollback();
            return NotFound();
        }

        articlesForWarehouseServ.BeginTrans();
        var entityArticleItemForWarehouse = ArticleItemForWarehouseFabric(entityMerchandise, Quantity);
        var resultArticleItemForWarehouse = articlesForWarehouseServ.Update(entityArticleItemForWarehouse);

        if (!resultArticleItemForWarehouse)
        {
            merchandiseServ.Rollback();
            articlesForWarehouseServ.Rollback();
            return NotFound();
        }

        articlesForSalesServ.BeginTrans();
        var entityArticleItemForSale = ArticleItemForSaleFabric(entityMerchandise, Price);
        var resultArticleItemForSales = articlesForSalesServ.Update(entityArticleItemForSale);
        if (!resultArticleItemForSales)
        {
            merchandiseServ.Rollback();
            articlesForWarehouseServ.Rollback();
            articlesForSalesServ.Rollback();
            return NotFound();
        }

        var entitysProductsForSales = productsForSalesServ.GetAllByMerchandiseId(entityMerchandise.Id!);
        if (!entitysProductsForSales.Any())
        {
            AllCommit();
            return Ok();
        }
        productsForSalesServ.BeginTrans();
        foreach (var item in entitysProductsForSales)
        {
            item.Packaging = entityMerchandise.Packaging;
            var resultProductForSales = productsForSalesServ.Update(item);
            if (!resultProductForSales)
            {
                merchandiseServ.Rollback();
                articlesForWarehouseServ.Rollback();
                articlesForSalesServ.Rollback();
                productsForSalesServ.Rollback();
                return NotFound();
            }
        }

        AllCommit(true);
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        var merchandiseId = new ObjectId(id);

        merchandiseServ.BeginTrans();
        var resultMerchandise = merchandiseServ.Delete(merchandiseId);
        if (!resultMerchandise)
        {
            return NotFound();
        }

        articlesForWarehouseServ.BeginTrans();
        var resultArticleItemForWarehouse = articlesForWarehouseServ.Delete(merchandiseId);
        if (!resultArticleItemForWarehouse)
        {
            merchandiseServ.Rollback();
            return NotFound();
        }

        articlesForSalesServ.BeginTrans();
        var resultArticleItemForSales = articlesForSalesServ.Delete(merchandiseId);
        if (!resultArticleItemForSales)
        {
            merchandiseServ.Rollback();
            articlesForWarehouseServ.Rollback();
            return NotFound();
        }

        var entitysProductsForSales = productsForSalesServ.GetAllByMerchandiseId(merchandiseId);
        if (!entitysProductsForSales.Any())
        {
            AllCommit();
            return Ok();
        }
        productsForSalesServ.BeginTrans();
        var resultProductForSales = productsForSalesServ.DeleteByMerchandiseId(merchandiseId);
        if (resultProductForSales != entitysProductsForSales.Count())
        {
            merchandiseServ.Rollback();
            articlesForWarehouseServ.Rollback();
            articlesForSalesServ.Rollback();
            productsForSalesServ.Rollback();
            return NotFound();
        }

        AllCommit(true);
        return Ok();
    }

    #region EXTRA
    ArticleItemForWarehouse ArticleItemForWarehouseFabric(MerchandiseItem entity, double quantity = 0)
    {
        return new ArticleItemForWarehouse
        {
            Quantity = quantity,
            MerchandiseName = entity.Name,
            MerchandiseId = entity.Id,
            Packaging = entity.Packaging
        };
    }

    ArticleItemForSale ArticleItemForSaleFabric(MerchandiseItem entity, double price = 0)
    {
        return new ArticleItemForSale
        {
            Price = price,
            MerchandiseName = entity.Name,
            MerchandiseId = entity.Id,
            Packaging = entity.Packaging
        };
    }

    void AllCommit(bool hasProducts = false)
    {
        merchandiseServ.Commit();
        articlesForWarehouseServ.Commit();
        articlesForSalesServ.Commit();
        if (hasProducts)
        {
            productsForSalesServ.Commit();
        }
    }
    #endregion
}

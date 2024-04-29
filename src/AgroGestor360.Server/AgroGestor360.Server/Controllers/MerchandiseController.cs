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

    public MerchandiseController(IMerchandiseInLiteDbService merchandiseService, IArticlesForWarehouseInLiteDbService articlesForWarehouseService, IArticlesForSalesInLiteDbService articlesForSalesIService)
    {
        merchandiseServ = merchandiseService;
        articlesForWarehouseServ = articlesForWarehouseService;
        articlesForSalesServ = articlesForSalesIService;
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

        merchandiseServ.Commit();
        articlesForWarehouseServ.Commit();
        articlesForSalesServ.Commit();
        return Ok(resultMerchandise);

        //var entityMerchandise = dTO.FromDTO1();
        //entityMerchandise.Id = ObjectId.NewObjectId();
        //var resultMerchandise = merchandiseServ.Insert(entityMerchandise);

        //if (string.IsNullOrEmpty(resultMerchandise))
        //{
        //    return NotFound();
        //}

        //var entityArticleItemForWarehouse = ArticleItemForWarehouseFabric(entityMerchandise);
        //var resultArticleItemForWarehouse = articlesForWarehouseServ.Insert(entityArticleItemForWarehouse);

        //if (string.IsNullOrEmpty(resultArticleItemForWarehouse))
        //{
        //    merchandiseServ.Delete(new ObjectId(resultMerchandise));
        //    return NotFound();
        //}

        //var entityArticleItemForSale = ArticleItemForSaleFabric(entityMerchandise);
        //var resultArticleItemForSales = articlesForSalesServ.Insert(entityArticleItemForSale);

        //if (string.IsNullOrEmpty(resultArticleItemForSales))
        //{
        //    merchandiseServ.Delete(new ObjectId(resultMerchandise));
        //    articlesForWarehouseServ.Delete(new ObjectId(resultArticleItemForWarehouse));
        //    return NotFound();
        ////}

        //return Ok(resultMerchandise);
    }
    
    [HttpPut]
    public IActionResult Put([FromBody] DTO1 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        merchandiseServ.BeginTrans();

        var entityMerchandise = dTO.FromDTO1();
        //var backupMerchandise = merchandiseServ.GetById(entityMerchandise.Id!);
        var resultMerchandise = merchandiseServ.Update(entityMerchandise);

        if (!resultMerchandise)
        {
            merchandiseServ.Rollback();
            return NotFound();
        }

        articlesForWarehouseServ.BeginTrans();
        var Quantity = articlesForWarehouseServ.GetById(entityMerchandise.Id!)?.Quantity ?? 0;
        var entityArticleItemForWarehouse = ArticleItemForWarehouseFabric(entityMerchandise, Quantity);
        var resultArticleItemForWarehouse = articlesForWarehouseServ.Update(entityArticleItemForWarehouse);

        if (!resultArticleItemForWarehouse)
        {
            merchandiseServ.Rollback();
            articlesForWarehouseServ.Rollback();
            //merchandiseServ.Update(backupMerchandise);
            return NotFound();
        }

        articlesForSalesServ.BeginTrans();
        var Price = articlesForSalesServ.GetById(entityMerchandise.Id!)?.Price ?? 0;
        var entityArticleItemForSale = ArticleItemForSaleFabric(entityMerchandise, Price);
        var resultArticleItemForSales = articlesForSalesServ.Update(entityArticleItemForSale);
        if (!resultArticleItemForSales)
        {
            merchandiseServ.Rollback();
            articlesForWarehouseServ.Rollback();
            articlesForSalesServ.Rollback();
            //merchandiseServ.Update(backupMerchandise);
            //articlesForWarehouseServ.Update(backupArticleItemForWarehouse);
            return NotFound();
        }

        merchandiseServ.Commit();
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
        var backupMerchandise = merchandiseServ.GetById(merchandiseId);
        var resultMerchandise = merchandiseServ.Delete(merchandiseId);

        if (resultMerchandise)
        {
            var backupArticleItemForWarehouse = articlesForWarehouseServ.GetById(merchandiseId);
            var resultArticleItemForWarehouse = articlesForWarehouseServ.Delete(merchandiseId);
            if (!resultArticleItemForWarehouse)
            {
                merchandiseServ.Insert(backupMerchandise);
                return NotFound();
            }

            var resultArticleItemForSales = articlesForSalesServ.Delete(merchandiseId);
            if (!resultArticleItemForSales)
            {
                merchandiseServ.Insert(backupMerchandise);
                articlesForWarehouseServ.Insert(backupArticleItemForWarehouse);
                return NotFound();
            }
        }

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
    #endregion
}

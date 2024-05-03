using AgroGestor360.Server.Models;
using AgroGestor360.Server.Services;
using AgroGestor360.Server.Tools.Extensions;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticlesForSalesController : ControllerBase
{
    readonly IArticlesForSalesInLiteDbService articlesForSalesServ;
    readonly IProductsForSalesInLiteDbService productsForSalesServ;

    public ArticlesForSalesController(IArticlesForSalesInLiteDbService articlesForSalesService, IProductsForSalesInLiteDbService productsForSalesService)
    {
        articlesForSalesServ = articlesForSalesService;
        productsForSalesServ = productsForSalesService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<DTO3>> GetAll()
    {
        var all = articlesForSalesServ.GetAll()?.Select(x => x.ToDTO3()) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("{id}")]
    public ActionResult<DTO3> GetById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        var find = articlesForSalesServ.GetById(new ObjectId(id));
        if (find is null)
        {
            NotFound();
        }

        var dTO = find!.ToDTO3();

        return Ok(dTO);
    }

    [HttpPut]
    public IActionResult Put([FromBody] DTO3_1 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        var found = articlesForSalesServ.GetById(new ObjectId(dTO.MerchandiseId));
        if (found is null)
        {
            return NotFound();
        }
        found.Price = dTO.Price;

        articlesForSalesServ.BeginTrans();
        bool resultArticleItemForSale = articlesForSalesServ.Update(found);
        if (!resultArticleItemForSale)
        {
            articlesForSalesServ.Rollback();
            return NotFound();
        }

        var entitysProductsForSales = productsForSalesServ.GetAllByMerchandiseId(found.MerchandiseId!);
        if (!entitysProductsForSales.Any())
        {
            articlesForSalesServ.Commit();
            return Ok();
        }
        productsForSalesServ.BeginTrans();
        foreach (var item in entitysProductsForSales)
        {
            item.ArticlePrice = item.ProductQuantity * dTO.Price;            
            var resultProductForSales = productsForSalesServ.Update(item);
            if (!resultProductForSales)
            {
                articlesForSalesServ.Rollback();
                productsForSalesServ.Rollback();
                return NotFound();
            }
        }

        articlesForSalesServ.Commit();
        productsForSalesServ.Commit();
        return Ok();
    }
}

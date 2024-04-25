using AgroGestor360.Server.Models;
using AgroGestor360.Server.Services;
using AgroGestor360.Server.Tools.Extensions;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsForSalesController : ControllerBase
{
    readonly IProductsForSalesInLiteDbService productsForSalesServ;

    public ProductsForSalesController(IProductsForSalesInLiteDbService productsForSalesService)
    {
        productsForSalesServ = productsForSalesService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = productsForSalesServ.Exist;

        return Ok(exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<DTO4>> GetAll()
    {
        var all = productsForSalesServ.GetAll()?.Select(x => x.ToDTO4()) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("getall1")]
    public ActionResult<IEnumerable<DTO4_1>> GetAll1()
    {
        var all = productsForSalesServ.GetAll()?.Select(x => x.ToDTO4_1()) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("get3/{id}")]
    public ActionResult<DTO4_3> GetProductOffering(string id)
    {
        var find = productsForSalesServ.GetById(new ObjectId(id));

        if (find is null)
        {
            NotFound();
        }

        var result = find!.ToDTO4_3();

        return !result.Offering?.Any() ?? true ? NotFound() : Ok(result);
    }

    [HttpGet("{id}")]
    public ActionResult<DTO4> Get(string id)
    {
        var find = productsForSalesServ.GetById(new ObjectId(id));

        if (find is null)
        {
            NotFound();
        }

        var dTO = find!.ToDTO4();

        return Ok(dTO);
    }

    [HttpPost]
    public ActionResult<string> Post([FromBody] DTO4 dTO)
    {
        var entity = dTO.FromDTO4();
        entity.Id = ObjectId.NewObjectId();

        return productsForSalesServ.Insert(entity);
    }

    [HttpPut]
    public IActionResult Put([FromBody] DTO4 dTO)
    {
        var updated = productsForSalesServ.Update(dTO.FromDTO4());

        return Ok(updated);
    }

    [HttpPut("send2")]
    public IActionResult ChangeQuantity([FromBody] DTO4_2 dTO)
    {
        ProductItemForSale entity = dTO.FromDTO4_2();
        var find = productsForSalesServ.GetById(entity.Id!);
        entity.Article = find.Article;
        entity.Offering = find.Offering;
        var updated = productsForSalesServ.Update(entity);

        return Ok(updated);
    }

    [HttpPut("send3")]
    public IActionResult ChangeOffering([FromBody] DTO4_3 dTO)
    {
        var find = productsForSalesServ.GetById(new ObjectId(dTO.Id));
        var offering = find.Offering;
        offering ??= [];
        offering.Insert(0, dTO.Offering!.Last());
        ProductItemForSale entity = new()
        {
            Id = find.Id,
            Name = find.Name,
            Quantity = find.Quantity,
            Article = find.Article,
            Offering = offering
        };
        var updated = productsForSalesServ.Update(entity);

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var deleted = productsForSalesServ.Delete(new ObjectId(id));

        return Ok(deleted);
    }
}

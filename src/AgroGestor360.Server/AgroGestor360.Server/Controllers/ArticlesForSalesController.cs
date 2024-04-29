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

    public ArticlesForSalesController(IArticlesForSalesInLiteDbService articlesForSalesService)
    {
        articlesForSalesServ = articlesForSalesService;
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

        return articlesForSalesServ.Update(found) ? Ok() : NotFound();
    }
}

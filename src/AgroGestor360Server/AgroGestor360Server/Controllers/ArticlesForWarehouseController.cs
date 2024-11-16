using AgroGestor360Server.Models;
using AgroGestor360Server.Services;
using AgroGestor360Server.Tools.Extensions;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360Server.Controllers;

[Route("[controller]")]
[ApiController]
public class ArticlesForWarehouseController : Controller
{
    readonly IArticlesForWarehouseInLiteDbService articlesForWarehouseServ;

    public ArticlesForWarehouseController(IArticlesForWarehouseInLiteDbService articlesForWarehouseService)
    {
        articlesForWarehouseServ = articlesForWarehouseService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<DTO2>> GetAll()
    {
        var all = articlesForWarehouseServ.GetAll()?.Select(x => x.ToDTO2()) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("{id}")]
    public ActionResult<DTO2> GetById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        var find = articlesForWarehouseServ.GetById(new ObjectId(id));
        if (find is null)
        {
            return NotFound();
        }
        var dto = find!.ToDTO2();
        return Ok(dto);
    }

    [HttpPut]
    public IActionResult Put([FromBody] DTO2_1 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        var found = articlesForWarehouseServ.GetById(new ObjectId(dTO.MerchandiseId));
        if (found is null)
        {
            return NotFound();
        }
        found.Quantity = dTO.Quantity;

        return articlesForWarehouseServ.Update(found) ? Ok() : NotFound();
    }
}

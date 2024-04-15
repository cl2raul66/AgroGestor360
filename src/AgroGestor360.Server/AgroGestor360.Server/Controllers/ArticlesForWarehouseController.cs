using AgroGestor360.Server.Models;
using AgroGestor360.Server.Sevices;
using AgroGestor360.Server.Tools.Extensions;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class ArticlesForWarehouseController : Controller
{
    readonly IArticlesForWarehouseInLiteDbService articlesForWarehouseServ;

    public ArticlesForWarehouseController(IArticlesForWarehouseInLiteDbService articlesForWarehouseService)
    {
        articlesForWarehouseServ = articlesForWarehouseService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = articlesForWarehouseServ.Exist;

        return Ok(exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<DTO2>> GetAll()
    {
        var all = articlesForWarehouseServ.GetAll()?.Select(x => x.ToDTO2()) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("all1")]
    public ActionResult<IEnumerable<DTO2_1>> GetAll1()
    {
        var all = articlesForWarehouseServ.GetAll()?.Select(x => x.ToDTO2_1()) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("allmerchandise")]
    public ActionResult<IEnumerable<DTO1>> GetAllMerchandise()
    {
        var all = articlesForWarehouseServ.GetAllMerchandise()?.Select(x => x.ToDTO1()) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("allcategories")]
    public ActionResult<IEnumerable<MerchandiseCategory>> GetAllCategories()
    {
        var all = articlesForWarehouseServ.GetAllCategories() ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("{id}")]
    public ActionResult<DTO2> GetById(string id)
    {
        var find = articlesForWarehouseServ.GetById(new ObjectId(id));
        if (find is null)
        {
            return NotFound();
        }
        var dto = find!.ToDTO2();
        return Ok(dto);
    }

    [HttpPost]
    public ActionResult<string> Post([FromBody] DTO2 dTO)
    {
        var entity = dTO.FromDTO2();
        entity.Id = ObjectId.NewObjectId();

        return articlesForWarehouseServ.Insert(entity);
    }

    [HttpPut]
    public IActionResult Put([FromBody] DTO2 dTO)
    {
        var result = articlesForWarehouseServ.Update(dTO.FromDTO2());

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var deleted = articlesForWarehouseServ.Delete(new ObjectId(id));

        return Ok(deleted);
    }
}

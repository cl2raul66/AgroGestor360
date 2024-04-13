using AgroGestor360.Server.Models;
using AgroGestor360.Server.Sevices;
using AgroGestor360.Server.Tools.Extensions;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class WarehouseController : Controller
{
    readonly IWarehouseForLitedbService warehouseServ;

    public WarehouseController(IWarehouseForLitedbService warehouseService)
    {
        warehouseServ = warehouseService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = warehouseServ.Exist;

        return Ok(exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<DTO2>> GetAll()
    {
        var all = warehouseServ.GetAll()?.Select(x => x.ToDTO2()) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("all1")]
    public ActionResult<IEnumerable<DTO2_1>> GetAll1()
    {
        var all = warehouseServ.GetAll()?.Select(x => x.ToDTO2_1()) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("allmerchandise")]
    public ActionResult<IEnumerable<DTO1>> GetAllMerchandise()
    {
        var all = warehouseServ.GetAllMerchandise()?.Select(x => x.ToDTO1()) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("allcategories")]
    public ActionResult<IEnumerable<MerchandiseCategory>> GetAllCategories()
    {
        var all = warehouseServ.GetAllCategories() ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("{id}")]
    public ActionResult<DTO2> GetById(string id)
    {
        var find = warehouseServ.GetById(new ObjectId(id));
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

        return warehouseServ.Insert(entity);
    }

    [HttpPut]
    public IActionResult Put([FromBody] DTO2 dTO)
    {
        var result = warehouseServ.Update(dTO.FromDTO2());

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var deleted = warehouseServ.Delete(new ObjectId(id));

        return Ok(deleted);
    }
}

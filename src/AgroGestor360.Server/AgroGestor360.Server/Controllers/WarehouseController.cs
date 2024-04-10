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
    readonly IMerchandiseForLitedbService merchandiseServ;
    readonly IMerchandiseCategoryForLitedbService merchandiseCategoryServ;

    public WarehouseController(IWarehouseForLitedbService warehouseService, IMerchandiseForLitedbService merchandiseService, IMerchandiseCategoryForLitedbService merchandiseCategoryService)
    {
        warehouseServ = warehouseService;
        merchandiseServ = merchandiseService;
        merchandiseCategoryServ = merchandiseCategoryService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = warehouseServ.Exist;

        return Ok(exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<WarehouseItemGetDTO>> GetAll()
    {
        var all = warehouseServ.GetAll()?.Select(x => {
            var dto = x.ToWarehouseItemDTO();
            var merchandise = merchandiseServ.GetById(x.MerchandiseId!);
            dto.Name = merchandise.Name;
            dto.Description = merchandise.Description;
            dto.MerchandiseCategory = merchandiseCategoryServ.GetById(merchandise.MerchandiseCategoryId!).Name;
            return dto;
        }) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("{id}")]
    public ActionResult<WarehouseItemGetDTO> GetById(string id)
    {
        var find = warehouseServ.GetById(new ObjectId(id));
        if (find is null)
        {
            return NotFound();
        }

        var dto = find!.ToWarehouseItemDTO();
        var merchandise = merchandiseServ.GetById(find.MerchandiseId!);
        dto.Name = merchandise.Name;
        dto.Description = merchandise.Description;
        dto.MerchandiseCategory = merchandiseCategoryServ.GetById(merchandise.MerchandiseCategoryId!).Name;

        return Ok(dto);
    }

    [HttpPost]
    public ActionResult<string> Post([FromBody] WarehouseItemSendDTO dTO)
    {
        var entity = dTO.ToWarehouseItem();
        entity.Id = ObjectId.NewObjectId();

        return warehouseServ.Insert(entity);
    }

    [HttpPut]
    public IActionResult Put([FromBody] WarehouseItemSendDTO dTO)
    {
        var result = warehouseServ.Update(dTO.ToWarehouseItem());

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var deleted = warehouseServ.Delete(new ObjectId(id));

        return Ok(deleted);
    }
}

using AgroGestor360.Server.Models;
using AgroGestor360.Server.Sevices;
using AgroGestor360.Server.Tools.Extensions;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class MerchandiseController : ControllerBase
{
    readonly IMerchandiseForLitedbService merchandiseServ;
    readonly IMerchandiseCategoryForLitedbService merchandiseCategoryServ;

    public MerchandiseController(IMerchandiseForLitedbService merchandiseService, IMerchandiseCategoryForLitedbService merchandiseCategoryService)
    {
        merchandiseServ = merchandiseService;
        merchandiseCategoryServ = merchandiseCategoryService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = merchandiseServ.Exist;

        return Ok(exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<MerchandiseDTO>> GetAllEnabled()
    {
        var all = merchandiseServ.GetAllEnabled()?.Select(x => {
            var dto = x.ToMerchandiseDTO();
            dto.MerchandiseCategory = merchandiseServ.GetById(x.MerchandiseCategoryId!).Name;
            return dto;
        }) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("byname/{name}")]
    public ActionResult<IEnumerable<MerchandiseDTO>> GetAllByName(string name)
    {
        var all = merchandiseServ.GetAllByName(name)?.Select(x => {
            var dto = x.ToMerchandiseDTO();
            dto.MerchandiseCategory = merchandiseServ.GetById(x.MerchandiseCategoryId!).Name;
            return dto;
        }) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("allcategories")]
    public ActionResult<IEnumerable<string>> GetAllCategories()
    {
        var all = merchandiseServ.GetAllCategories()?.Select(x => x.ToString()) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("{id}")]
    public ActionResult<MerchandiseDTO> GetById(string id)
    {
        var find = merchandiseServ.GetById(new ObjectId(id));
        if (find is null)
        {
            return NotFound();
        }

        var dto = find.ToMerchandiseDTO();
        dto.MerchandiseCategory = merchandiseServ.GetById(find.MerchandiseCategoryId!).Name;
        return Ok(dto);
    }

    [HttpPost]
    public ActionResult<string> Post([FromBody] MerchandiseDTO dTO)
    {
        var entity = dTO.ToMerchandise();
        entity.Id = ObjectId.NewObjectId();
        var result = merchandiseServ.Insert(entity);
        return Ok(result);
    }

    [HttpPut]
    public IActionResult Put([FromBody] MerchandiseDTO dTO)
    {
        var updated = merchandiseServ.Update(dTO.ToMerchandise());

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var deleted = merchandiseServ.Delete(new ObjectId(id));

        return Ok(deleted);
    }
}

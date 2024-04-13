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

    public MerchandiseController(IMerchandiseForLitedbService merchandiseService)
    {
        merchandiseServ = merchandiseService;
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

    [HttpGet("allcategories")]
    public ActionResult<IEnumerable<MerchandiseCategory>> GetAllCategories()
    {
        var all = merchandiseServ.GetAllCategories();
        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("{id}")]
    public ActionResult<DTO1> GetById(string id)
    {
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
        var entity = dTO.FromDTO1();
        entity.Id = ObjectId.NewObjectId();
        var result = merchandiseServ.Insert(entity);
        return Ok(result);
    }

    [HttpPut]
    public IActionResult Put([FromBody] DTO1 dTO)
    {
        var entity = dTO.FromDTO1();
        var result = merchandiseServ.Update(entity);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var deleted = merchandiseServ.Delete(new ObjectId(id));
        return Ok(deleted);
    }
}

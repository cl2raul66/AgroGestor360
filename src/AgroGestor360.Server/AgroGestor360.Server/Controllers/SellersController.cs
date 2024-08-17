using AgroGestor360.Server.Models;
using AgroGestor360.Server.Services;
using AgroGestor360.Server.Tools.Extensions;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class SellersController : ControllerBase
{
    readonly ISellersInLiteDbService sellersServ;

    public SellersController(ISellersInLiteDbService sellersService)
    {
        sellersServ = sellersService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = sellersServ.Exist;

        return Ok(exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<DTO6>> GetAll()
    {
        var all = sellersServ.GetAll()?.Select(x => x.ToDTO6()) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("{id}")]
    public ActionResult<DTO6_2?> GetById(string id)
    {
        var find = sellersServ.GetById(new ObjectId(id));

        return find is null ? NotFound() : Ok(find!.ToDTO6_2());
    }

    [HttpPost]
    public ActionResult<string> Post([FromBody] DTO6_1 dTO)
    {
        var entity = dTO.FromDTO6_1();
        entity.Id = ObjectId.NewObjectId();

        var result = sellersServ.Insert(entity);

        return string.IsNullOrEmpty(result) ? NotFound() : Ok(result);
    }

    [HttpPut]
    public ActionResult<bool> Put([FromBody] DTO6_2 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }
        var entity = dTO.FromDTO6_2();
        if (entity is null)
        {
            return NotFound();
        }
        var result = sellersServ.Update(entity);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public ActionResult<bool> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }
        var result = sellersServ.Delete(new ObjectId(id));

        return Ok(result);
    }
}

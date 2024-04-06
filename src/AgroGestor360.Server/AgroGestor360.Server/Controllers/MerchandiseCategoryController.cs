using AgroGestor360.Server.Models;
using AgroGestor360.Server.Sevices;
using AgroGestor360.Server.Tools.Extensions;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class MerchandiseCategoryController : Controller
{
    readonly IMerchandiseCategoryForLitedbService merchandiseCategoryServ;

    public MerchandiseCategoryController(IMerchandiseCategoryForLitedbService merchandiseCategoryService)
    {
        merchandiseCategoryServ = merchandiseCategoryService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = merchandiseCategoryServ.Exist;

        return Ok(exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<MerchandiseCategoryDTO>> Get()
    {
        var all = merchandiseCategoryServ.GetAllEnabled()?.Select(x => x.ToMerchandiseCategoryDTO()) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("{id}")]
    public ActionResult<MerchandiseCategoryDTO> Get(string id)
    {
        var bank = merchandiseCategoryServ.GetById(new ObjectId(id));

        return bank is null ? NotFound() : Ok(bank!.ToMerchandiseCategoryDTO());
    }

    [HttpPost]
    public ActionResult<string> Post([FromBody] MerchandiseCategoryDTO dTO)
    {
        dTO.Id = ObjectId.NewObjectId().ToString();
        return merchandiseCategoryServ.Insert(dTO.ToMerchandiseCategory());
    }

    [HttpPut]
    public IActionResult Put([FromBody] MerchandiseCategoryDTO dTO)
    {
        var updated = merchandiseCategoryServ.Update(dTO.ToMerchandiseCategory());

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var deleted = merchandiseCategoryServ.Delete(new ObjectId(id));

        return Ok(deleted);
    }
}

using AgroGestor360.Server.Sevices;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using vCardLib.Models;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class SellersController : ControllerBase
{
    private readonly ISellersForLitedbService sellerServ;

    public SellersController(ISellersForLitedbService sellerService)
    {
        sellerServ = sellerService;
    }

    [HttpGet("exist")]
    public ActionResult<bool> Exist() => Ok(sellerServ.Exist);

    [HttpGet]
    public ActionResult<IEnumerable<vCard>> GetAll()
    {
        var all = sellerServ.GetAll();
        if (!all?.Any() ?? true)
        {
            return NotFound();
        }

        return Ok(all);
    }

    [HttpGet("byname/{name}")]
    public IActionResult GetAllByName(string name)
    {
        return Ok(sellerServ.GetAllByName(name));
    }

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var card = sellerServ.GetById(id);
        return card is null ? NotFound() : Ok(card);
    }

    [HttpPost]
    public ActionResult<bool> Post([FromBody] vCard card)
    {
        card.Uid = ObjectId.NewObjectId().ToString();
        var id = sellerServ.Insert(card);
        return Ok(!string.IsNullOrEmpty(id));
    }

    [HttpPut]
    public ActionResult<bool> Put([FromBody] vCard card)
    {
        var result = sellerServ.Update(card);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public ActionResult<bool> Delete(string id)
    {
        var result = sellerServ.Delete(id);
        return Ok(result);
    }
}

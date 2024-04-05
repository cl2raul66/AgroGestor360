using AgroGestor360.Server.Sevices;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using vCardLib.Models;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomersForLitedbService customersServ;

    public CustomersController(ICustomersForLitedbService customersService)
    {
        customersServ = customersService;
    }

    [HttpGet("exist")]
    public ActionResult<bool> Exist() => Ok(customersServ.Exist);

    [HttpGet]
    public ActionResult<IEnumerable<vCard>> GetAll()
    {
        var all = customersServ.GetAll();
        if (!all?.Any() ?? true)
        {
            return NotFound();
        }

        return Ok(all);
    }

    [HttpGet("byname/{name}")]
    public IActionResult GetAllByName(string name)
    {
        return Ok(customersServ.GetAllByName(name));
    }

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var card = customersServ.GetById(id);
        return card is null ? NotFound() : Ok(card);
    }

    [HttpPost]
    public ActionResult<bool> Post([FromBody] vCard card)
    {
        card.Uid = ObjectId.NewObjectId().ToString();
        var id = customersServ.Insert(card);
        return Ok(!string.IsNullOrEmpty(id));
    }

    [HttpPut]
    public ActionResult<bool> Put([FromBody] vCard card)
    {
        var result = customersServ.Update(card);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public ActionResult<bool> Delete(string id)
    {
        var result = customersServ.Delete(id);
        return Ok(result);
    }
}

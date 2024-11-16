using AgroGestor360Server.Models;
using AgroGestor360Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360Server.Controllers;

[ApiController]
[Route("[controller]")]
public class DiscountsCustomersController : ControllerBase
{
    readonly IDiscountsInLiteDbService discountsServ;

    public DiscountsCustomersController(IDiscountsInLiteDbService discountsService)
    {
        discountsServ = discountsService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = discountsServ.Exist;

        return Ok(exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<DiscountForCustomer>> GetAll()
    {
        var all = discountsServ.GetAll();

        return all is not null || all!.Any() ? Ok(all) : NotFound();
    }

    [HttpGet("{id}")]
    public ActionResult<DiscountForCustomer?> GetById(int id)
    {
        if (id < 1)
        {
            return BadRequest();
        }
        var found = discountsServ.GetById(id);

        return found is null ? NotFound() : Ok(found);
    }

    [HttpPost]
    public ActionResult<int> Post(DiscountForCustomer newEntity)
    {
        if (newEntity is null)
        {
            return BadRequest();
        }

        var result = discountsServ.Insert(newEntity);

        return result > 0 ? Ok(result) : NotFound();
    }

    [HttpPut]
    public ActionResult<bool> Update(DiscountForCustomer entity)
    {
        if (entity is null)
        {
            return BadRequest();
        }
        var result = discountsServ.Update(entity);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public ActionResult<bool> Delete(int id)
    {
        if (id < 1)
        {
            return BadRequest();
        }
        var result = discountsServ.Delete(id);

        return Ok(result);
    }
}

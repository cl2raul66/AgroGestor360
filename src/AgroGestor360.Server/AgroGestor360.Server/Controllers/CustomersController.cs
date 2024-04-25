using AgroGestor360.Server.Models;
using AgroGestor360.Server.Services;
using AgroGestor360.Server.Tools.Extensions;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController : ControllerBase
{
    readonly ICustomersInLiteDbService customersServ;
    readonly IEnumerable<CustomerDiscountClass> AllDiscount;

    public CustomersController(ICustomersInLiteDbService customersService, IConfiguration configuration)
    {
        customersServ = customersService;
        AllDiscount = configuration.GetSection("DefaultClientClasses")?.GetSection("Types").Get<IEnumerable<CustomerDiscountClass>>() ?? [];
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = customersServ.Exist;

        return Ok(exist);
    }

    [HttpGet("getalldiscount")]
    public ActionResult<IEnumerable<CustomerDiscountClass>> GetAllDiscount() => !AllDiscount?.Any() ?? true ? NotFound() : Ok(AllDiscount);

    [HttpGet("getallwithdiscount")]
    public ActionResult<IEnumerable<DTO5_1>> GetAllWithDiscount()
    {
        var customers = customersServ.GetAllWithDiscount()?.Select(x => x.ToDTO5_1()) ?? [];

        return !customers?.Any() ?? true ? NotFound() : Ok(customers);
    }

    [HttpGet("getallwithoutdiscount")]
    public ActionResult<IEnumerable<DTO5_1>> GetAllWithoutDiscount()
    {
        var customers = customersServ.GetAllWithoutDiscount()?.Select(x => x.ToDTO5_1()) ?? [];

        return !customers?.Any() ?? true ? NotFound() : Ok(customers);
    }

    [HttpGet("{id}")]
    public ActionResult<DTO5_3?> GetById(string id)
    {
        var find = customersServ.GetById(new ObjectId(id));

        return find is null ? NotFound() : Ok(find!.ToDTO5_3());
    }

    [HttpPost]
    public ActionResult<string> Post([FromBody] DTO5_2 dTO)
    {
        var entity = dTO.FromDTO5_2();
        entity.Id = ObjectId.NewObjectId();

        var result = customersServ.Insert(entity);

        return string.IsNullOrEmpty(result) ? NotFound(): Ok(result);
    }

    [HttpPut]
    public ActionResult<bool> Update([FromBody] DTO5_3 dTO)
    {
        var entity = dTO.FromDTO5_3();
        if (entity is null)
        {
            return NoContent();
        }
        var result = customersServ.Update(entity);

        return Ok(result);
    }

    [HttpPut("updatediscount")]
    public ActionResult<bool> UpdateDiscount([FromBody] DTO5_4 dTO)
    {
        var entity = customersServ.GetById(new ObjectId(dTO.CustomerId));
        if (entity is null)
        {
            return NoContent();
        }
        if (dTO.DiscountId > 0)
        {
            entity.Discount = AllDiscount.FirstOrDefault(x => x.Id == dTO.DiscountId);
            if (entity.Discount is null)
            {
                return NotFound();
            }
        }
        else
        {
            entity.Discount = null;
        }
        var result = customersServ.Update(entity);

        return Ok(result);
    }
}

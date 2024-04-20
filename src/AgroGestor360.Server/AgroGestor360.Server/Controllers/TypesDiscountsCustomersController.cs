using AgroGestor360.Server.Models;
using AgroGestor360.Server.Sevices;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class TypesDiscountsCustomersController : ControllerBase
{
    readonly ITypesDiscountsCustomersService typesDiscountsCustomersServ;
    readonly IConfiguration config;

    readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public TypesDiscountsCustomersController(ITypesDiscountsCustomersService typesDiscountsCustomersService, IConfiguration configuration)
    {
        typesDiscountsCustomersServ = typesDiscountsCustomersService;
        config = configuration;
    }

    [HttpGet("Exist")]
    public ActionResult<bool> Exist()
    {
        return Ok(typesDiscountsCustomersServ.Exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<CustomerDiscountClass>> GetAll()
    {
        var all = typesDiscountsCustomersServ.GetAll();

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("{id}")]
    public ActionResult<CustomerDiscountClass?> GetById(int id)
    {
        var find = typesDiscountsCustomersServ.GetById(id);
        if (find is null)
        {
            NotFound();
        }
        return Ok(find);
    }

    [HttpPost]
    public ActionResult<int> Post(CustomerDiscountClass entity)
    {
        var id = typesDiscountsCustomersServ.Insert(entity);
        if (id <= 0)
        {
            return BadRequest();
        }
        return Ok(id);
    }

    [HttpPut]
    public ActionResult<bool> Update(CustomerDiscountClass entity)
    {
        var updated = typesDiscountsCustomersServ.Update(entity);

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var deleted = typesDiscountsCustomersServ.Delete(id);

        return Ok(deleted);
    }

    [HttpGet("getalldefault")]
    public ActionResult<IEnumerable<CustomerDiscountClass>> GetAllDefault()
    {
        var defaultClientClassesSection = config.GetSection("DefaultClientClasses");

        var defaultDiscounts = defaultClientClassesSection.GetSection("types").Get<IEnumerable<CustomerDiscountClass>>() ?? [];

        return Ok(defaultDiscounts);
    }

    [HttpGet("getbyiddefault/{id}")]
    public ActionResult<CustomerDiscountClass?> GetByIdDefault(int id)
    {
        var defaultClientClassesSection = config.GetSection("DefaultClientClasses");

        var defaultDiscounts = defaultClientClassesSection.GetSection("types").Get<IEnumerable<CustomerDiscountClass>>() ?? [];

        var find = defaultDiscounts!.FirstOrDefault(discount => discount.Id == id);

        if (find is null)
        {
            return NotFound();
        }
        return Ok(find);
    }

}

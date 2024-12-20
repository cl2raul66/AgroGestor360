﻿using AgroGestor360Server.Models;
using AgroGestor360Server.Services;
using AgroGestor360Server.Tools.Extensions;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360Server.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController : ControllerBase
{
    readonly ICustomersInLiteDbService customersServ;
    readonly IDiscountsInLiteDbService discountsServ;
    readonly ILineCreditsInLiteDbService lineCreditsServ;

    public CustomersController(ICustomersInLiteDbService customersService, IDiscountsInLiteDbService discountsService, ILineCreditsInLiteDbService lineCreditsService)
    {
        customersServ = customersService;
        discountsServ = discountsService;
        lineCreditsServ = lineCreditsService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = customersServ.Exist;

        return Ok(exist);
    }

    [HttpGet("getalldiscount")]
    public ActionResult<IEnumerable<DiscountForCustomer>> GetAllDiscount()
    {
        var all = customersServ.GetAllDiscount();
        return all is not null && all.Any() ? Ok(all) : NotFound();
    }

    [HttpGet]
    public ActionResult<IEnumerable<DTO5_1>> GetAll()
    {
        var all = customersServ.GetAll()?.Select(x => x.ToDTO5_1()) ?? [];

        return all is not null && all.Any() ? Ok(all) : NotFound();
    }

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
    public ActionResult<DTO5_1?> GetById(string id)
    {
        var find = customersServ.GetById(new ObjectId(id));

        return find is null ? NotFound() : Ok(find!.ToDTO5_1());
    }

    [HttpGet("GetDTO5_3/{id}")]
    public ActionResult<DTO5_3?> GetDTO5_3ById(string id)
    {
        var find = customersServ.GetById(new ObjectId(id));

        return find is null ? NotFound() : Ok(find!.ToDTO5_3());
    }

    [HttpPost]
    public ActionResult<string> Post(DTO5_2 dTO)
    {
        var entity = dTO.FromDTO5_2();
        entity.Id = ObjectId.NewObjectId();

        var result = customersServ.Insert(entity);

        return string.IsNullOrEmpty(result) ? NotFound(): Ok(result);
    }

    [HttpPut]
    public ActionResult<bool> Update(DTO5_3 dTO)
    {
        var entity = dTO.FromDTO5_3();
        if (entity is null)
        {
            return NoContent();
        }
        var result = customersServ.Update(entity);

        return Ok(result);
    }
    
    [HttpPut("updatecredit")]
    public ActionResult<bool> UpdateCredit(DTO5_5 dTO)
    {
        if (dTO is null)
        {
            return NoContent();
        }

        var entity = customersServ.GetById(new ObjectId(dTO.CustomerId));
        if (entity is null)
        {
            return NotFound();
        }
        entity.Credit = dTO.Credit;

        var result = customersServ.Update(entity);

        return Ok(result);
    }

    [HttpPut("updatediscount")]
    public ActionResult<bool> UpdateDiscount(DTO5_4 dTO)
    {
        var entity = customersServ.GetById(new ObjectId(dTO.CustomerId));
        if (entity is null)
        {
            return NoContent();
        }
        if (dTO.DiscountId > 0)
        {
            entity.Discount = discountsServ.GetById(dTO.DiscountId);
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

    [HttpDelete("{id}")]
    public ActionResult<bool> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }
        var result = customersServ.Delete(new ObjectId(id));

        return Ok(result);
    }
}

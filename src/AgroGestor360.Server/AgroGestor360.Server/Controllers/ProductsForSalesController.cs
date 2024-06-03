using AgroGestor360.Server.Models;
using AgroGestor360.Server.Services;
using AgroGestor360.Server.Tools.Extensions;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsForSalesController : ControllerBase
{
    readonly IProductsForSalesInLiteDbService productsForSalesServ;

    public ProductsForSalesController(IProductsForSalesInLiteDbService productsForSalesService)
    {
        productsForSalesServ = productsForSalesService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = productsForSalesServ.Exist;

        return Ok(exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<DTO4>> GetAll()
    {
        var all = productsForSalesServ.GetAll()?.Select(x => x.ToDTO4()) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("getallbymerchandiseid/{merchandiseId}")]
    public ActionResult<IEnumerable<DTO4>> GetAllByMerchandiseId(string merchandiseId)
    {
        if (string.IsNullOrEmpty(merchandiseId))
        {
            return BadRequest();
        }

        var all = productsForSalesServ.GetAllByMerchandiseId(new ObjectId(merchandiseId))?.Select(x => x.ToDTO4()) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("offers/{id}")]
    public ActionResult<IEnumerable<ProductOffering>> GetOffersById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        var found = productsForSalesServ.GetById(new ObjectId(id));
        if (!found?.Offering?.Any() ?? true)
        {
            return NotFound();
        }

        return Ok(found!.Offering);
    }

    [HttpGet("{id}")]
    public ActionResult<DTO4> GetById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        var find = productsForSalesServ.GetById(new ObjectId(id));
        if (find is null)
        {
            return NotFound();
        }

        var dTO = find!.ToDTO4();

        return Ok(dTO);
    }

    [HttpPost]
    public ActionResult<string> Post([FromBody] DTO4_1 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }
        var entity = dTO.FromDTO4_1();
        entity.Id = ObjectId.NewObjectId();

        var result = productsForSalesServ.Insert(entity);

        return string.IsNullOrEmpty(result) ? NotFound() : Ok(result);
    }

    [HttpPut("send2")]
    public IActionResult ChangeQuantity([FromBody] DTO4_2 dTO)
    {
        if (string.IsNullOrEmpty(dTO.Id))
        {
            return BadRequest();
        }

        var found = productsForSalesServ.GetById(new ObjectId(dTO.Id));
        if (found is null)
        {
            return NotFound();
        }
        found.ProductQuantity = dTO.ProductQuantity;
        var result = productsForSalesServ.Update(found);

        return result ? Ok() : NotFound();
    }

    [HttpPut("send3")]
    public IActionResult ChangeOffering([FromBody] DTO4_3 dTO)
    {
        if (dTO is null)
        {
            return BadRequest();
        }

        var found = productsForSalesServ.GetById(new ObjectId(dTO.Id));
        if (found is null)
        {
            return NotFound();
        }

        if (found.Offering is null || found.Offering.Length == 0)
        {
            found.Offering = [dTO.Offer!];
        }
        else
        {
            var list = found.Offering.ToList();
            list.Add(dTO.Offer!);
            found.Offering = list.ToArray();
        }

        var result = productsForSalesServ.Update(found);

        return result ? Ok() : NotFound();
    }

    [HttpPut("send4")]
    public IActionResult ChangeOfferingRemove([FromBody] DTO4_4 dTO)
    {
        if (string.IsNullOrEmpty(dTO.Id) || dTO.OfferId < 1)
        {
            return BadRequest();
        }

        var found = productsForSalesServ.GetById(new ObjectId(dTO.Id));
        if (found is null)
        {
            return NotFound();
        }
        found.Offering = found.Offering?.Where(x => x.Id != dTO.OfferId).ToArray();

        var result = productsForSalesServ.Update(found);

        return result ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }
        var result = productsForSalesServ.Delete(new ObjectId(id));

        return result ? Ok() : NotFound();
    }
}

using AgroGestor360.Server.Models;
using AgroGestor360.Server.Sevices;
using AgroGestor360.Server.Tools.Extensions;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticlesForSalesController : ControllerBase
{
    readonly IArticlesForSalesInLiteDbService articlesForSalesServ;

    public ArticlesForSalesController(IArticlesForSalesInLiteDbService articlesForSalesService)
    {
        articlesForSalesServ = articlesForSalesService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = articlesForSalesServ.Exist;

        return Ok(exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<DTO3>> GetAll()
    {
        var all = articlesForSalesServ.GetAll()?.Select(x => x.ToDTO3()) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("all1")]
    public ActionResult<IEnumerable<DTO2_1>> GetAll1()
    {
        var all = articlesForSalesServ.GetAll()?.Select(x => x.ToDTO3_1()) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("allcategories")]
    public ActionResult<IEnumerable<MerchandiseCategory>> GetAllCategories()
    {
        var all = articlesForSalesServ.GetAllCategories() ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("allmerchandise")]
    public ActionResult<IEnumerable<DTO1>> GetAllMerchandise()
    {
        var all = articlesForSalesServ.GetAllMerchandise()?.Select(x => x.ToDTO1()) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("{id}")]
    public ActionResult<DTO3> Get(string id)
    {
        var find = articlesForSalesServ.GetById(new ObjectId(id));

        if (find is null)
        {
            NotFound();
        }

        var dTO = find!.ToDTO3();

        return Ok(dTO);
    }

    [HttpPost]
    public ActionResult<string> Post([FromBody] DTO3 dTO)
    {
        var entity = dTO.FromDTO3();
        entity.Id = ObjectId.NewObjectId();

        return articlesForSalesServ.Insert(entity);
    }

    [HttpPut]
    public IActionResult Put([FromBody] DTO3 dTO)
    {
        var updated = articlesForSalesServ.Update(dTO.FromDTO3());

        return Ok(updated);
    }

    [HttpPut("changeprice")]
    public IActionResult ChangePrice([FromBody] DTO3 dTO)
    {
        ArticleItemForSale entity = dTO.FromDTO3();
        entity.Merchandise = articlesForSalesServ.GetById(entity.Id!).Merchandise;
        var updated = articlesForSalesServ.Update(entity);

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var deleted = articlesForSalesServ.Delete(new ObjectId(id));

        return Ok(deleted);
    }
}

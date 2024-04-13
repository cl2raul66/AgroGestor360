using AgroGestor360.Server.Models;
using AgroGestor360.Server.Sevices;
using AgroGestor360.Server.Tools.Extensions;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticlesController : ControllerBase
{
    readonly IArticlesForLitedbService articlesServ;
    readonly IMerchandiseForLitedbService merchandiseServ;

    public ArticlesController(IArticlesForLitedbService articlesService, IMerchandiseForLitedbService merchandiseService)
    {
        articlesServ = articlesService;
        merchandiseServ = merchandiseService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = articlesServ.Exist;

        return Ok(exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<ArticleDTO>> GetAllEnabled()
    {
        var all = articlesServ.GetAllEnabled()?.Select(x =>
        {
            var dto = x.ToArticleDTO();
            var entity = merchandiseServ.GetById(x.MerchandiseId!);
            dto.Merchandise = $"{entity.Name} | {entity.Packaging!.Value} {entity.Packaging!.Unit}";
            return dto;
        }) ?? [];

        return !all?.Any() ?? true ? NotFound() : Ok(all);
    }

    [HttpGet("{id}")]
    public ActionResult<ArticleDTO> Get(string id)
    {
        var find = articlesServ.GetById(new ObjectId(id));

        if (find is null)
        {
            NotFound();
        }

        var dTO = find!.ToArticleDTO();
        dTO.Merchandise = merchandiseServ.GetById(find!.MerchandiseId!).Name;

        return Ok(dTO);
    }

    [HttpPost]
    public ActionResult<string> Post([FromBody] ArticleDTO dTO)
    {
        var entity = dTO.ToArticle();
        entity.Id = ObjectId.NewObjectId();

        return articlesServ.Insert(entity);
    }

    [HttpPut]
    public IActionResult Put([FromBody] ArticleDTO dTO)
    {
        var updated = articlesServ.Update(dTO.ToArticle());

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var deleted = articlesServ.Delete(new ObjectId(id));

        return Ok(deleted);
    }
}

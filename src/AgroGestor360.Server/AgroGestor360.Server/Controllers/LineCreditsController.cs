using AgroGestor360.Server.Models;
using AgroGestor360.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class LineCreditsController : ControllerBase
{
    readonly ILineCreditsInLiteDbService lineCreditsServ;

    public LineCreditsController(ILineCreditsInLiteDbService lineCreditsService)
    {
        lineCreditsServ = lineCreditsService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = lineCreditsServ.Exist;

        return Ok(exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<LineCreditItem>> GetAll()
    {
        var all = lineCreditsServ.GetAll();

        return all is not null && all!.Any() ? Ok(all) : NotFound();
    }

    [HttpGet("{id}")]
    public ActionResult<LineCreditItem?> GetById(int id)
    {
        if (id < 1)
        {
            return BadRequest();
        }
        var found = lineCreditsServ.GetById(id);

        return found is null ? NotFound() : Ok(found);
    }

    [HttpPost]
    public ActionResult<int> Post(LineCreditItem newEntity)
    {
        if (newEntity is null)
        {
            return BadRequest();
        }

        var result = lineCreditsServ.Insert(newEntity);

        return result > 0 ? Ok(result) : NotFound();
    }

    [HttpPut]
    public ActionResult<bool> Update(LineCreditItem entity)
    {
        if (entity is null)
        {
            return BadRequest();
        }
        var result = lineCreditsServ.Update(entity);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public ActionResult<bool> Delete(int id)
    {
        if (id < 1)
        {
            return BadRequest();
        }
        var result = lineCreditsServ.Delete(id);

        return Ok(result);
    }


    [HttpGet("timeslimit")]
    public ActionResult<IEnumerable<TimeLimitForCredit>> GetAllTimeLimitForCredit()
    {
        var all = lineCreditsServ.GetAll1();

        return all is not null && all!.Any() ? Ok(all) : NotFound();
    }

    [HttpGet("timeslimit/{id}")]
    public ActionResult<TimeLimitForCredit?> GetByIdTimeLimitForCredit(int id)
    {
        if (id < 1)
        {
            return BadRequest();
        }
        var found = lineCreditsServ.GetById(id);

        return found is null ? NotFound() : Ok(found);
    }

    [HttpPost("timeslimit")]
    public ActionResult<int> PostTimeLimitForCredit(TimeLimitForCredit entity)
    {
        if (entity is null)
        {
            return BadRequest();
        }

        var result = lineCreditsServ.Insert1(entity);

        return result > 0 ? Ok(result) : NotFound();
    }

    [HttpDelete("timeslimit/{id}")]
    public ActionResult<bool> DeleteTimeLimitForCredit(int id)
    {
        if (id < 1)
        {
            return BadRequest();
        }

        var result = lineCreditsServ.Delete1(id);

        return Ok(result);
    }


    [HttpGet("defaulttimeslimit")]
    public ActionResult<TimeLimitForCredit?> GetDefault()
    {
        var all = lineCreditsServ.GetDefault();

        return all is null ? NotFound() : Ok(all);
    }

    [HttpPost("defaulttimeslimit")]
    public IActionResult SetDefault(TimeLimitForCredit entity)
    {
        if (entity is null)
        {
            return BadRequest();
        }

        var result = lineCreditsServ.SetDefault(entity);

        return result ? Ok() : NotFound();
    }
}

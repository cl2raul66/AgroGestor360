using AgroGestor360.Server.Models;
using AgroGestor360.Server.Sevices;
using Microsoft.AspNetCore.Mvc;
using LiteDB;
using AgroGestor360.Server.Tools.Extensions;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class BanksController : ControllerBase
{
    private readonly IBanksForLitedbService banksServ;

    public BanksController(IBanksForLitedbService banksService)
    {
        banksServ = banksService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = banksServ.Exist;

        return Ok(exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<BankDTO>> Get()
    {
        var banks = banksServ.GetAll()?.Select(x => x.ToBankDTO()) ?? [];

        return !banks?.Any() ?? true ? NotFound() : Ok(banks);
    }

    [HttpGet("{id}")]
    public ActionResult<BankDTO> Get(string id)
    {
        var bank = banksServ.GetById(new ObjectId(id));

        return bank is null ? NotFound() : Ok(bank!.ToBankDTO());
    }

    [HttpPost]
    public ActionResult<string> Post([FromBody] BankDTO bank)
    {
        bank.Id = ObjectId.NewObjectId().ToString();
        return banksServ.Insert(bank.ToBank());
    }

    [HttpPut]
    public IActionResult Put([FromBody] BankDTO bank)
    {
        var updated = banksServ.Update(bank.ToBank());

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var deleted = banksServ.Delete(new ObjectId(id));

        return Ok(deleted);
    }
}

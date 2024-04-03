using AgroGestor360.Server.Models;
using AgroGestor360.Server.Sevices;
using AgroGestor360.Server.Tools.Extensions;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class BankAccountsController : ControllerBase
{
    readonly IBankAccountsForLitedbService bankAccountsServ;
    readonly IBanksForLitedbService banksServ;

    public BankAccountsController(IBankAccountsForLitedbService bankAccountsService, IBanksForLitedbService banksService)
    {
        bankAccountsServ = bankAccountsService;
        banksServ = banksService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = bankAccountsServ.Exist;

        return Ok(exist);
    }

    [HttpGet("{number}")]
    public ActionResult<BankAccountDTO> Get(string number)
    {
        var find = bankAccountsServ.GetByNumber(number);
        if (find is null)
        {
            return NotFound();
        }

        var dto = find!.ToBankAccountDTO();
        dto.BankName = banksServ.GetById(find.BankId!).Name;

        return Ok(dto);
    }


    [HttpGet]
    public ActionResult<IEnumerable<BankAccountDTO>> Get()
    {
        var all = bankAccountsServ.GetAllEnabled();
        if (!all?.Any() ?? true)
        {
            return NotFound();
        }

        var allDTO = all!.Select(x =>
        {
            var dto = x.ToBankAccountDTO();
            dto.BankName = banksServ.GetById(x.BankId!).Name;
            return dto;
        });

        return Ok(allDTO);
    }

    [HttpPost]
    public ActionResult<bool> Post([FromBody] BankAccountDTO dTO)
    {
        var entity = dTO.ToBankAccount();
        entity.Id = ObjectId.NewObjectId();
        entity.BankId = banksServ.GetIdByName(dTO.BankName!);

        string id = bankAccountsServ.Insert(entity);

        return Ok(!string.IsNullOrEmpty(id));
    }

    [HttpPut]
    public IActionResult Put([FromBody] BankAccountDTO dTO)
    {
        var entity = dTO.ToBankAccount();
        var find = bankAccountsServ.GetByNumber(dTO.Number!);
        if (find is null)
        {
            return NotFound();
        }
        entity.Id = find!.Id;

        var result = bankAccountsServ.Update(entity);

        return Ok(result);
    }

    [HttpDelete("{number}")]
    public IActionResult Delete(string number)
    {
        var find = bankAccountsServ.GetByNumber(number);
        var deleted = bankAccountsServ.Delete(find!.Id!);

        return Ok(deleted);
    }
}

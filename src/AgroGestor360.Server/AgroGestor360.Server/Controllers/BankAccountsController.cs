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

    public BankAccountsController(IBankAccountsForLitedbService bankAccountsService)
    {
        bankAccountsServ = bankAccountsService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = bankAccountsServ.Exist;

        return Ok(exist);
    }

    [HttpGet("exists/{number}")]
    public IActionResult CheckExistenceByNumber(string number)
    {
        bool exist = bankAccountsServ.ThatBankAccountNumberExists(number);

        return Ok(exist);
    }

    [HttpGet("{number}")]
    public ActionResult<BankAccountDTO> Get(string number)
    {
        var findBankAccount = bankAccountsServ.GetByNumber(number);

        return findBankAccount is null ? NotFound() : Ok(findBankAccount!.ToBankAccountDTO());
    }


    [HttpGet]
    public ActionResult<IEnumerable<BankAccountDTO>> Get()
    {
        var banks = bankAccountsServ.GetAll()?.Select(x => x.ToBankAccountDTO()) ?? [];

        return !banks?.Any() ?? true ? NotFound() : Ok(banks);
    }

    [HttpPost]
    public ActionResult<bool> Post([FromBody] BankAccountDTO bankAccount)
    {
        var newBankAccount = bankAccount.ToBankAccount();
        newBankAccount.Id = ObjectId.NewObjectId();
        string id = bankAccountsServ.Insert(newBankAccount);
        return Ok(!string.IsNullOrEmpty(id));
    }

    [HttpPut]
    public IActionResult Put([FromBody] BankAccountDTO bank)
    {
        var findBankAccount = bankAccountsServ.GetByNumber(bank.Number!);
        var updatedBankAccount = bank.ToBankAccount();
        updatedBankAccount.Id = findBankAccount!.Id;
        var result = bankAccountsServ.Update(updatedBankAccount);

        return Ok(result);
    }

    [HttpDelete("{number}")]
    public IActionResult Delete(string number)
    {
        var findBankAccount = bankAccountsServ.GetByNumber(number);
        var deleted = bankAccountsServ.Delete(findBankAccount!.Id!);

        return Ok(deleted);
    }
}

using AgroGestor360.Server.Models;
using AgroGestor360.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class BankAccountsController : ControllerBase
{
    readonly IBankAccountsLiteDbService bankAccountsService;

    public BankAccountsController(IBankAccountsLiteDbService bankAccountsService)
    {
        this.bankAccountsService = bankAccountsService;
    }

    [HttpGet("exist")]
    public IActionResult CheckExistence()
    {
        bool exist = bankAccountsService.Exist;

        return Ok(exist);
    }

    [HttpGet]
    public ActionResult<IEnumerable<BankAccount>> GetAll()
    {
        var bankAccounts = bankAccountsService.GetAll();

        return !bankAccounts?.Any() ?? true ? NotFound() : Ok(bankAccounts);
    }

    [HttpGet("{id}")]
    public ActionResult<BankAccount> GetById(string id)
    {
        var find = bankAccountsService.GetById(id);

        return find is null ? NotFound() : Ok(find);
    }

    [HttpPost]
    public ActionResult<string> Post([FromBody] BankAccount bankAccount)
    {
        var result = bankAccountsService.Insert(bankAccount);

        return string.IsNullOrEmpty(result) ? NotFound() : Ok(result);
    }

    [HttpPut]
    public ActionResult<bool> Put([FromBody] BankAccount bankAccount)
    {
        var result = bankAccountsService.Update(bankAccount);

        return !result ? NotFound() : Ok(result);
    }

    [HttpDelete("{id}")]
    public ActionResult<bool> Delete(string id)
    {
        var result = bankAccountsService.Delete(id);

        return !result ? NotFound() : Ok(result);
    }
}

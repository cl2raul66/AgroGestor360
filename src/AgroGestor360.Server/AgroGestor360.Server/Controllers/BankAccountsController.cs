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
    

    
    [HttpGet("BankItem")]
    public ActionResult<IEnumerable<BankItem>> GetAllBankItem()
    {
        var bankAccounts = bankAccountsService.GetBankItemAll();

        return !bankAccounts?.Any() ?? true ? NotFound() : Ok(bankAccounts);
    }

    [HttpGet("BankItem/{id}")]
    public ActionResult<BankItem> GetBankItemById(string id)
    {
        var find = bankAccountsService.GetBankItemById(id);

        return find is null ? NotFound() : Ok(find);
    }

    [HttpPost("BankItem")]
    public ActionResult<string> PostBankItem(BankItem bankAccount)
    {
        var result = bankAccountsService.InsertBankItem(bankAccount);

        return string.IsNullOrEmpty(result) ? NotFound() : Ok(result);
    }

    [HttpPut("BankItem")]
    public ActionResult<bool> PutBankItem(BankItem bankAccount)
    {
        var result = bankAccountsService.UpdateBankItem(bankAccount);

        return !result ? NotFound() : Ok(result);
    }

    [HttpDelete("BankItem/{id}")]
    public ActionResult<bool> DeleteBankItem(string id)
    {
        var result = bankAccountsService.DeleteBankItem(id);

        return !result ? NotFound() : Ok(result);
    }
}

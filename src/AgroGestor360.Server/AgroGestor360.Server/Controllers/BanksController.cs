using AgroGestor360.Server.Models;
using AgroGestor360.Server.Sevices;
using Microsoft.AspNetCore.Mvc;
using LiteDB;
using AgroGestor360.Server.Tools.Extensions;

namespace AgroGestor360.Server.Controllers
{
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
        public ActionResult<IEnumerable<Bank>> Get()
        {
            var banks = banksServ.GetAll();

            return !banks?.Any() ?? true ? NotFound() : Ok(banks);
        }

        [HttpGet("{id}")]
        public ActionResult<Bank> Get(string id)
        {
            var bank = banksServ.GetById(new ObjectId(id));

            return bank is null ? NotFound() : Ok(bank);
        }

        [HttpPost]
        public ActionResult<string> Post([FromBody] BankDTO bank)
        {
            return banksServ.Insert(bank.ToBank());
        }

        [HttpPut]
        public IActionResult Put([FromBody] BankDTO bank)
        {
            var updated = banksServ.Update(bank.ToBank());

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(ObjectId id)
        {
            var deleted = banksServ.Delete(id);

            return Ok(deleted);
        }
    }
}

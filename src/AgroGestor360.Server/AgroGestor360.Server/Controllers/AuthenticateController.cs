using AgroGestor360.Server.Tools.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IConfiguration configuration) : Controller
{
    private readonly IConfiguration _configuration = configuration;

    [HttpPost]
    public IActionResult Authenticate([FromBody] string tokenauth)
    {
        if (string.IsNullOrEmpty(tokenauth) && string.IsNullOrWhiteSpace(tokenauth))
        {
            return BadRequest();
        }

        string? username = _configuration["User:Username"];
        string? password = _configuration["User:Password"];
        string? salt = _configuration["License:ClientId"];

        string token = HashHelper.GenerateHash(username! + password! + salt!);

        return token == tokenauth ? Ok() : Unauthorized();
    }
}

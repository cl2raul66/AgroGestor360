using AgroGestor360.Server.Tools.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IConfiguration configuration) : Controller
{
    private readonly IConfiguration cfg = configuration;

    [HttpPost]
    public IActionResult Authenticate(string tokenauth)
    {
        if (string.IsNullOrEmpty(tokenauth) && string.IsNullOrWhiteSpace(tokenauth))
        {
            return BadRequest();
        }

        string? username = cfg["User:Username"];
        string? password = cfg["User:Password"];
        string? salt = cfg["Organization:Id"];

        string token = HashHelper.GenerateHash(username! + password! + salt!);

        return token == tokenauth ? Ok() : Unauthorized();
    }
}

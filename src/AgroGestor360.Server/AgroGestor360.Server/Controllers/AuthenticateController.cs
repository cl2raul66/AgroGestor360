using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace AgroGestor360.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthenticateController(IConfiguration configuration) : Controller
{
    private readonly IConfiguration _configuration = configuration;

    [HttpPost]
    public IActionResult Authenticate(string tokenauth)
    {
        var hashes = tokenauth.Split(':')?.Select(x => x.Trim()).ToArray();
        if (hashes is null && hashes!.Length != 2 && string.IsNullOrEmpty(hashes[0]) && string.IsNullOrEmpty(hashes[1]))
        {
            return BadRequest();
        }

        string? username = _configuration["User:Username"];
        string? password = _configuration["User:Password"];
        string? salt = _configuration["License:ClientId"];

        string token = GenerateHash(username! + password! + salt!);

        return token == tokenauth ? Ok() : Unauthorized();
    }

    private string GenerateHash(string input)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        StringBuilder sb = new();
        for (int i = 0; i < bytes.Length; i++)
        {
            sb.Append(bytes[i].ToString("x2"));
        }
        return sb.ToString();
    }
}

using AgroGestor360Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360Server.Controllers;

[Route("[controller]")]
[ApiController]
public class OrganizationController(IConfiguration configuration) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;

    [HttpGet]
    public IActionResult GetOrganization()
    {
        var organization = _configuration.GetSection("Organization").Get<Organization>();
        return Ok(organization);
    }

    [HttpGet("Id")]
    public IActionResult GetOrganizationId()
    {
        var organization = _configuration["Organization:Id"];
        return Ok(organization);
    }
}

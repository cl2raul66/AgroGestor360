using AgroGestor360.Server.Models;
using AgroGestor360.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class ReconciliationController : ControllerBase
{
    readonly IReconciliationInLiteDbService reconciliationInLiteDbServ;
    readonly IAuditedReconciliationInLiteDbService auditedReconciliationInLiteDbServ;

    public ReconciliationController(IReconciliationInLiteDbService reconciliationInLiteDbService, IAuditedReconciliationInLiteDbService auditedReconciliationInLiteDbService)
    {
        reconciliationInLiteDbServ = reconciliationInLiteDbService;
        auditedReconciliationInLiteDbServ = auditedReconciliationInLiteDbService;
    }

    #region POLICIES
    [HttpGet("policy")]
    public IActionResult GetPolicy()
    {
        var policy = reconciliationInLiteDbServ.GetPolicy();
        if (policy == null)
        {
            return NotFound();
        }
        return Ok(policy);
    }

    [HttpGet("haspolicy")]
    public IActionResult HasPolicy()
    {
        bool hasPolicy = reconciliationInLiteDbServ.HasPolitic();
        return Ok(hasPolicy);
    }

    [HttpPost("policy")]
    public IActionResult InsertPolicy([FromBody] ReconciliationPolicy entity)
    {
        if (entity == null)
        {
            return BadRequest("Invalid policy entity.");
        }

        reconciliationInLiteDbServ.BeginTrans();
        try
        {
            int id = reconciliationInLiteDbServ.InsertPolicy(entity);
            reconciliationInLiteDbServ.Commit();
            return CreatedAtAction(nameof(GetPolicy), new { id }, entity);
        }
        catch (Exception)
        {
            reconciliationInLiteDbServ.Rollback();
            return StatusCode(500, "An error occurred while inserting the policy.");
        }
    }

    [HttpPut("policy")]
    public IActionResult UpdatePolicy([FromBody] ReconciliationPolicy entity)
    {
        if (entity is null)
        {
            return BadRequest();
        }

        reconciliationInLiteDbServ.BeginTrans();
        try
        {
            bool success = reconciliationInLiteDbServ.UpdatePolicy(entity);
            reconciliationInLiteDbServ.Commit();
            return success ? Ok() : NotFound();
        }
        catch (Exception)
        {
            reconciliationInLiteDbServ.Rollback();
            return NotFound();
        }
    }
    #endregion

    #region CASH RECONCILIATION

    #endregion

    #region AUDITED CASH RECONCILIATION

    #endregion
}

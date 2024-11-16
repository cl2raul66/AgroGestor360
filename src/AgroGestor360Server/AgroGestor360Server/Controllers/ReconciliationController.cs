using AgroGestor360Server.Models;
using AgroGestor360Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgroGestor360Server.Controllers;

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
        return Ok(reconciliationInLiteDbServ.GetPolicy());
    }

    [HttpGet("haspolicy")]
    public IActionResult HasPolicy()
    {
        bool hasPolicy = reconciliationInLiteDbServ.HasPolitic();
        return Ok(hasPolicy);
    }

    [HttpPost("policy")]
    public IActionResult InsertPolicy(ReconciliationPolicy entity)
    {
        if (entity is null)
        {
            return BadRequest();
        }

        reconciliationInLiteDbServ.BeginTrans();
        try
        {
            int id = reconciliationInLiteDbServ.InsertPolicy(entity);
            reconciliationInLiteDbServ.Commit();
            return Ok(id);
        }
        catch (Exception)
        {
            reconciliationInLiteDbServ.Rollback();
            return NotFound();
        }
    }

    [HttpPut("policy")]
    public IActionResult UpdatePolicy(ReconciliationPolicy entity)
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
            return Ok(success);
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

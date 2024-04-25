using AgroGestor360.Server.Models;
using AgroGestor360.Server.Services;
using AgroGestor360.Server.Tools.Hubs;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AgroGestor360.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientDeviceController : ControllerBase
{
    private readonly IClientdeviceForLitedbService clientdeviceForLitedbServ; 
    private readonly IHubContext<NotificationHub> hubCtx;

    public ClientDeviceController(IClientdeviceForLitedbService service, IHubContext<NotificationHub> hubContext)
    {
        clientdeviceForLitedbServ = service;
        hubCtx = hubContext;
    }

    [HttpGet("Exist")]
    public ActionResult<bool> Exist()
    {
        return Ok(clientdeviceForLitedbServ.Exist);
    }

    [HttpGet("{id}")]
    public ActionResult<IEnumerable<ClientDevice>> GetAllById(string id)
    {
        return Ok(clientdeviceForLitedbServ.GetAllById(new ObjectId(id)));
    }

    [HttpPost]
    public async Task<ActionResult<string>> Register(ClientDevice clientDevice)
    {
        var id = clientdeviceForLitedbServ.Insert(clientDevice);
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }
        await hubCtx.Groups.AddToGroupAsync(id, "anonymous");

        //await hubCtx.Clients.All.SendAsync("ReceiveMessage", $"{OperationType.Create}:{nameof(ClientDevice)}:{id}");
        return Ok(id);
    }

    [HttpPut]
    public async Task<ActionResult<bool>> Update(ClientDevice clientDevice)
    {
        var result = clientdeviceForLitedbServ.Update(clientDevice);
        if (result)
        {
            //await hubCtx.Clients.All.SendAsync("ReceiveMessage", $"{OperationType.Update}:{nameof(ClientDevice)}:{clientDevice.Id}");
            await Task.CompletedTask;
            return Ok(result);
        }
        await Task.CompletedTask;
        return BadRequest();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(string id)
    {
        var result = clientdeviceForLitedbServ.Delete(new ObjectId(id));
        if (result)
        {
            //await hubCtx.Clients.All.SendAsync("ReceiveMessage", $"{OperationType.Delete}:{nameof(ClientDevice)}:{id}");
            await Task.CompletedTask;
            return Ok(result);
        }
        await Task.CompletedTask;
        return BadRequest();
    }
}

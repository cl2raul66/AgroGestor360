using AgroGestor360.Server.Tools.Enums;
using Microsoft.AspNetCore.SignalR;

namespace AgroGestor360.Server.Tools.Hubs;

public class NotificationHub : Hub
{
    public static ServerStatus Status = ServerStatus.Running;

    public async Task SendStatusMessage(ServerStatus status)
    {
        Status = status;
        await Clients.All.SendAsync("ReceiveStatusMessage", status);
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("ReceiveStatusMessage", Status);
        await base.OnConnectedAsync();
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Caller.SendAsync("JoinedGroup", groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        await Clients.Caller.SendAsync("LeftGroup", groupName);
    }

    public async Task SendExpiredQuotationMessage(string[] quotationCodes)
    {
        await Clients.Group("QuotationView").SendAsync("ReceiveExpiredQuotationMessage", quotationCodes);
    }

    public async Task SendExpiredOrderMessage(string[] ordersCodes)
    {
        await Clients.Group("OrderView").SendAsync("ReceiveExpiredOrderMessage", ordersCodes);
    }
}

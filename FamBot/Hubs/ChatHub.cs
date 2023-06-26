using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;

namespace FamBot.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
    public void RecieveLogEvent(string[] groups, string[] userIds, Serilog.Sinks.)
}
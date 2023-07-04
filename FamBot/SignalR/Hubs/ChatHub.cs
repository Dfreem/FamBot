using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;

namespace FamBot.SignalR.Hubs;

public class ChatHub : Hub<IChatClient>
{
    public async Task SendMessage(string color, string message) =>
        await Clients.All.RecieveMessage(color, message);
}
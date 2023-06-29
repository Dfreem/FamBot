using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;

namespace FamBot.Hubs;

public class ChatHub : Hub<IChatClient>
{
    public async Task SendMessage(string user, string message) =>
        await Clients.All.RecieveMessage(user, message);
}
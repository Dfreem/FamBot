
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FamBot.Data.Services;
using FamBot.Hubs;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using DSharpPlus.Interactivity.Extensions;

public class Slash : ApplicationCommandModule
{
    public IHubContext<ChatHub, IChatClient> ChatContext { get; set; } = default!;

}
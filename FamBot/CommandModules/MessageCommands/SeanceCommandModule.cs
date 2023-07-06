#region usings
using DSharpPlus;
using DSharpPlus.Entities;
using FamBot.Data.Services;
using FamBot.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using FamBot.Shared.Helpers;
#endregion

namespace FamBot.CommandModules.MessageCommands;

public class SeanceCommandModule : BaseCommandModule
{
    private const string _botColor = "#395f7";
    // SignalR Hub
    public IHubContext<ChatHub, IChatClient> ChatContext { get; set; } = default!;

    public AiService? AiApi { get; set; }

    public Serilog.ILogger Logger { get; set; } = default!;

    [Command("seance")]
    public async Task LetsHaveASeance(CommandContext context)
    {
        // give the illusion of work being done
        await context.TriggerTypingAsync();

        // go have a seance with a robot, come back with the result.
        var seanceResponse = await AiApi!.HaveASeance();
       
        DiscordButtonComponent[] buttons = await DiscordHelper.GetDiscordButtonsAsync(context.Client);
        DiscordMessageBuilder builder = new();
        builder.AddEmbed(new DiscordEmbedBuilder()
            .WithTitle("Seance")
            .WithColor(DiscordColor.Grayple)
            .WithDescription(seanceResponse.Choices.First().Message.Content))
            .AddComponents(buttons);
        _ = await context.Channel.SendMessageAsync(builder);

    }


}

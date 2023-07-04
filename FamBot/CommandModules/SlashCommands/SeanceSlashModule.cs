#region usings
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity.Extensions;
using Serilog;
using FamBot.Data;
using FamBot.Data.Services;
using FamBot.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
#endregion

namespace FamBot.CommandModules.SlashCommands;

public class SeanceSlashModule : ApplicationCommandModule
{
    // SignalR Hub
    public IHubContext<ChatHub, IChatClient> ChatContext { get; set; } = default!;

    public AiService AiApi { get; set; } = default!;

    public Serilog.ILogger Logger { get; set; } = default!;

    // Discord
    public DiscordClient DClient { get; set; } = default!;

    [SlashCommand("LetsHaveASeance", "Well Becca... Let's have a seance.")]
    public async Task LetsHaveASeance(InteractionContext context)
    {
        // Ack the message to let the user know we are working on it.
        await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        
        // go have a seance with a robot, come back with the result.
        var seanceResponse = await AiApi.HaveASeance();
        await ChatContext.Clients.All.RecieveMessage( "#33AA59", seanceResponse);

        // GPT will respond with options for buttons inside of '[ ]'.
        // Split them away from the bulk of the response.
        //var promptParts = seanceResponse.Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
        //string responseContent = promptParts[0];
        //var buttonParts = promptParts[1..];
        //var buttons = DiscordUtility.ButtonsFromGPTResponse(buttonParts);

        var builder = new DiscordWebhookBuilder()
            .WithContent(seanceResponse);
            //.AddComponents(buttons);

        // first send the written response.
        DiscordMessage sent = await context.EditResponseAsync(builder);

        // TODO this part doesn't work
        var recieved = await sent.GetNextMessageAsync();
        await ChatContext.Clients.All.RecieveMessage("#33AA59", recieved.ToString()!);
        Log.Information(recieved.Result.Content);
     }
}
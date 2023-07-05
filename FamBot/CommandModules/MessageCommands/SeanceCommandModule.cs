#region usings
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Serilog;
using FamBot.Data;
using FamBot.Data.Services;
using FamBot.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.ObjectModel;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Security.Cryptography.X509Certificates;
using OpenAI.ObjectModels.ResponseModels;
#endregion

namespace FamBot.CommandModules.MessageCommands;

public class SeanceCommandModule : BaseCommandModule
{
    // SignalR Hub
    public IHubContext<ChatHub, IChatClient> ChatContext { get; set; } = default!;

    public AiService? AiApi { get; set; }

    public Serilog.ILogger Logger { get; set; } = default!;

    // Discord
    public DiscordClient DClient { get; set; } = default!;

    [Command("seance")]
    public async Task LetsHaveASeance(CommandContext context)
    {
        // give the illusion of work being done
        await context.TriggerTypingAsync();
        
        // go have a seance with a robot, come back with the result.
        var seanceResponse = await AiApi!.HaveASeance();
        await HavingSeance(context, seanceResponse);

    }
    public async Task HavingSeance(CommandContext context, ChatCompletionCreateResponse response)
    {
        var interactivity = 
        DiscordEmoji[] emojiOptions =
        {
            DiscordEmoji.FromName(context.Client, ":one:"),
            DiscordEmoji.FromName(context.Client, ":two:"),
            DiscordEmoji.FromName(context.Client, ":three:"),
            DiscordEmoji.FromName(context.Client, ":four:")
        };
        var builder = new DiscordMessageBuilder()
            .AddEmbed(new DiscordEmbedBuilder()
            .WithTitle("choose your response")
            .WithDescription(
            $"{response.Choices.First().Message.Content}\nReact with your answer" +
                $"{emojiOptions[0]}" +
                $"{emojiOptions[1]}" +
                $"{emojiOptions[2]}" +
                $"{emojiOptions[3]}"));
        //var sent = await context.

        await ChatContext.Clients.All.RecieveMessage("#33AA59",
            response.Choices.First().Message.Content);

        //var message = await context.Message.RespondAsync(builder);
        var message = await context.Channel.SendMessageAsync(builder);
        foreach (var emoji in emojiOptions)
        {
            await message.CreateReactionAsync(emoji);
        }
        //ReadOnlyCollection<Reaction> reactions = await message.CollectReactionsAsync(new TimeSpan(0, 0, 15));
       var reactions = await 

        var emojiName = reactions.MaxBy(r => r.Total);
        string emojiNumber = (emojiName?.Emoji.Name) switch
        {
            ":one:" => "1",
            ":two:" => "2",
            ":three:" => "3",
            ":four:" => "4",
            _ => ""
        };
        if (emojiNumber != string.Empty)
        {
            // Something funny going on --------stopped here ----------------------
            var seanceResponse = await AiApi.RespondToSeanceAsync(emojiNumber);
            await HavingSeance(context, seanceResponse);
        }
        else
        {
            await context.RespondAsync("Your spirit has vanished");
        }
    }
}

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.ComponentModel;
using FamBot.Data.Services;
using Ical.Net.DataTypes;
using Ical.Net.CalendarComponents;
using DSharpPlus.Interactivity.Extensions;
using NodaTime.Extensions;
using Serilog;
using FamBot.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FamBot.CommandModules;

public class Slash : ApplicationCommandModule
{
    public CalendarService CalService { get; set; } = default!;
    public IHubContext<ChatHub, IChatClient> ChatContext { get; set; } = default!;

    [SlashCommand("todo", "add a todo item to Devins calendar")]
    public async Task Remind(InteractionContext context,
        [Option("what", "what is the thing that needs done?")] string todo,
        [Option("when", "today? tomorrow? Thursday?")] DayOfWeek doOn,
        [Option("description", "do you have anymore details?")] string desc = "No description"
        )
    {
        string reminder = await CalService.AddTodoAsync(todo, desc, doOn);
        await ChatContext.Clients.All.RecieveMessage(context.Member.ToString(), reminder);
        await context.CreateResponseAsync(reminder);
    }

    [SlashCommand("get_week", "display the todo's listed for this week.")]
    public async Task GetThisWeek(InteractionContext context)
    {
        string thisWeek = await CalService.GetThisWeek();
        await context.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder()
            {
                Content = $"This Week: {thisWeek}"
            });
        Log.Logger.Information($"From GetThisWeek in Slash.cs/line45\n{thisWeek}");
    }

    [SlashCommand("get_all_events", "retrieve all of the events on devins calendar")]
    public async Task GetAllEvents(InteractionContext context)
    {
        string events = await CalService.GetTodos();
        var interactivity = context.Client.GetInteractivity();
        var pages = interactivity.GeneratePagesInEmbed(events);
        await context.Channel.SendPaginatedMessageAsync(context.Member, pages);
        await ChatContext.Clients.All.RecieveMessage(context.Member.ToString(), events);
        Log.Logger.Information($"events in GetAllEvents, Slash.cs/line61\n{events}");
    }
}
//[Flags]
//public enum RepeatOn
//{
//    [ChoiceName("daily")]
//    Daily = 1,
//    [ChoiceName("once a week")]
//    Weekly = 2,
//    [ChoiceName("more than once a week")]
//    MoreThanOnce = 4,
//    [ChoiceName("one time reminder")]
//    OneShot = 8
//}


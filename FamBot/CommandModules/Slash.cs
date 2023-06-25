using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.ComponentModel;
using FamBot.Data.Services;
using Ical.Net.DataTypes;
using Ical.Net.CalendarComponents;

namespace FamBot.CommandModules;

public class Slash : ApplicationCommandModule
{
    public CalendarService CalService { get; set; }

    [SlashCommand("remind", "add a reminder to Devins calendar")]
    public async Task Remind(InteractionContext context, [Option("Task", "what is the thing that needs done?")] string todo)
    {
        await context.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder()
            {
                Content = await CalService.AddReminderAsync(new Todo()
                {
                    Description = todo,

                })
            });
    }

    [SlashCommand("get_week", "display the todo's listed for this week.")]
    public async Task GetThisWeek(InteractionContext context)
    {
        string thisWeek = await CalService.GetThisWeek();
        await context.CreateResponseAsync($"This Week: {thisWeek}");
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


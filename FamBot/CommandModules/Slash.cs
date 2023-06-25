using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace FamBot.CommandModules;

public class Slash : ApplicationCommandModule
{
    [SlashCommand("remind", "add a reminder to Devins calendar")]
    public async Task Remind(InteractionContext context,
        [Option("day", "the day to do the thing")] DayOfWeek day,
        [Option("Task", "what is the thing that needs done?")] string todo,
        [Option("repeat", "should the reminder repeat? how often?")] RepeatOn frequency)
    {
        await context.CreateResponseAsync(InteractionResponseType
            .ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent($"Here is the info you submitted:\n Day: {day}\nTask{todo}\nRepeat Frequency: {frequency}"));
    }
}
[Flags]
public enum RepeatOn
{
    Sunday = 1,
    Monday = 2,
    Tuesday = 4,
    Wednesday = 8,
    Thursday = 16,
    Friday = 32,
    Saturday = 64
}
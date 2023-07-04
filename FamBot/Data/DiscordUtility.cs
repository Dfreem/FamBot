using DSharpPlus;
using DSharpPlus.Entities;
using Serilog;
using System.Text.RegularExpressions;

namespace FamBot.Data;

public static class DiscordUtility
{
    public static DiscordComponent[] ButtonsFromGPTResponse(string[] parts)
    {
        
        List<DiscordButtonComponent> buttons = new();
        foreach (string part in parts)
        {
            var newbutton = new DiscordButtonComponent(ButtonStyle.Primary, $"{DateTime.Now.Microsecond}", part);
            Log.Information(part, newbutton);
            buttons.Add(newbutton);
        }
        return buttons.ToArray();
    }
}


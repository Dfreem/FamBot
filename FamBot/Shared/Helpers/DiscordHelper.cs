using System;
using DSharpPlus;
using DSharpPlus.Entities;

namespace FamBot.Shared.Helpers;

public static class DiscordHelper
{
    public static async Task<DiscordButtonComponent[]> GetDiscordButtonsAsync(DiscordClient client)
    {
        return await Task.FromResult(new DiscordButtonComponent[]{
            new DiscordButtonComponent(ButtonStyle.Primary, "one", DiscordEmoji.FromName(client, ":one:")),
            new DiscordButtonComponent(ButtonStyle.Primary, "two", DiscordEmoji.FromName(client, ":two:")),
            new DiscordButtonComponent(ButtonStyle.Primary, "three", DiscordEmoji.FromName(client, ":three:")),
            new DiscordButtonComponent(ButtonStyle.Primary, "four", DiscordEmoji.FromName(client, ":four:"))
        });

    }
}


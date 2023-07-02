
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FamBot.Data.Services;
using FamBot.Hubs;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using DSharpPlus.Interactivity.Extensions;

namespace FamBot.CommandModules
{
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
            await context.DeferAsync();

            var seanceResponse = await AiApi.HaveASeance();

            await context.EditResponseAsync(new DiscordWebhookBuilder()
            {
                Content = seanceResponse
            });
        }
    }
}

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
        
        
        
        // Discord
        public DiscordClient DClient { get; set; } = default!;

        [SlashCommand("Lets Have a Seance", "Well Becca... Let's have a seance. Call this command to speak with a random deceased historical figure.")]
        public async Task LetsHaveASeance()
        {

        }
    }
}
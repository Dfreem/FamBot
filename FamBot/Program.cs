
#region Usings
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity;
using DSharpPlus;
using Microsoft.AspNetCore.ResponseCaching;
using FamBot;
using FamBot.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Serilog.Sinks.SystemConsole;
using Serilog.Sinks.File;
using Microsoft.Extensions.Configuration.UserSecrets;
using OpenAI.Extensions;
using OpenAI.Managers;
using FamBot.SignalR.Hubs;
using FamBot.CommandModules.MessageCommands;
using DSharpPlus.Entities;
#endregion

var builder = WebApplication.CreateBuilder(args);

#region configure services

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Information()
              .WriteTo.Console()
              .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
              .CreateLogger();

builder.Services.AddTransient(services => new AiService(builder.Configuration));

string promptRoot = Path.GetFullPath(Environment.CurrentDirectory);

//builder.Configuration.AddKeyPerFile();

#region ----------- Discord Config -----------

var discord = new DiscordClient(new DiscordConfiguration()
{
    Token = builder.Configuration["Discord:Token"],
    TokenType = TokenType.Bot,
    Intents = DiscordIntents.All,
    MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Information,
    HttpTimeout = Timeout.InfiniteTimeSpan
});

discord.UseInteractivity(new InteractivityConfiguration()
{
    PollBehaviour = PollBehaviour.KeepEmojis,
    Timeout = TimeSpan.FromSeconds(500)
});
#endregion


#endregion


var app = builder.Build();

#region register Discord commands

var slash = discord.UseSlashCommands(new SlashCommandsConfiguration()
{
    Services = app.Services
});

var message = discord.UseCommandsNext(new CommandsNextConfiguration()
{
    Services = app.Services,
    StringPrefixes = new[]
    {
        "lets have a sceance",
        DiscordEmoji.FromName(discord, ":woman_genie:"),
        DiscordEmoji.FromName(discord, ":ghost:")
    }
});
message.RegisterCommands<SeanceCommandModule>();

// Uncomment to use CalendarSlashModule
//commands.RegisterCommands<CalendarSlashModule>(1055294750095331439);

#endregion

app.UseResponseCompression();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.MapBlazorHub();
app.MapHub<ChatHub>("/chathub");
app.MapFallbackToPage("/_Host");

await discord.ConnectAsync();
app.Run();

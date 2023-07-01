using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity;
using DSharpPlus;
using Microsoft.AspNetCore.ResponseCaching;
using FamBot.Hubs;
using FamBot;
using FamBot.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using DSharpPlus.Interactivity.Extensions;
using FamBot.CommandModules;
using DSharpPlus.SlashCommands;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Serilog.Sinks.SystemConsole;
using Serilog.Sinks.File;
using Microsoft.Extensions.Configuration.UserSecrets;

var builder = WebApplication.CreateBuilder(args);

#region configure services

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

Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Information()
              .WriteTo.Console()
              .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
              .CreateLogger();

Log.Information("Logger Initializing: Program.cs");

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});
//builder.Services.AddSingleton<CalendarService>(new CalendarService("Data/icalexport.ics"));
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDirectoryBrowser();

#endregion


var app = builder.Build();
var commands = discord.UseSlashCommands(new SlashCommandsConfiguration()
{
    Services = app.Services
});

//commands.RegisterCommands<Slash>(1055294750095331439);

await discord.ConnectAsync();
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

app.Run();

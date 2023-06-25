using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity;
using DSharpPlus;
using Microsoft.AspNetCore.ResponseCaching;
using FamBot.Hubs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using DSharpPlus.Interactivity.Extensions;
using FamBot.CommandModules;
using DSharpPlus.SlashCommands;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var discord = new DiscordClient(new DiscordConfiguration()
{
    Token = builder.Configuration["Discord:Token"],
    TokenType = TokenType.Bot,
    Intents = DiscordIntents.All,
    MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug
});
discord.UseInteractivity(new InteractivityConfiguration()
{
    PollBehaviour = PollBehaviour.KeepEmojis,
    Timeout = TimeSpan.FromSeconds(60)
});

var commands = discord.UseSlashCommands();
commands.RegisterCommands<Slash>(1055294750095331439);

await discord.ConnectAsync();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();
app.UseResponseCompression();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapHub<ChatHub>("/chathub");
app.MapFallbackToPage("/_Host");

app.Run();

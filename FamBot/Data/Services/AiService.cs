using Microsoft.AspNetCore.Mvc;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using Microsoft.AspNetCore.SignalR.Protocol;
using Serilog;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.ResponseModels;
using System.Text.Json;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using FamBot.Shared.Helpers;
using System;
using FamBot.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FamBot.Data.Services;
public class AiService
{
    private const string _botColor = "#395f7";
    private readonly string _savePath = "Data/Prompts/SeanceLog.json";
    private readonly string _promptPath = "Data/Prompts/SeancePrompt.txt";
    private readonly OpenAIService _aiService;
    private readonly string _seancePrompt;
    private readonly IHubContext<ChatHub, IChatClient> _chatContext;


    public AiService(IConfiguration config, IServiceProvider services)
    {
        _chatContext = services.GetRequiredService<IHubContext<ChatHub, IChatClient>>();
        _aiService = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = config["OpenAi:Key"]!
        });
        var promptPath = Path.Combine(Path.GetPathRoot("~")!, _promptPath);
        _seancePrompt = File.ReadAllText(promptPath);
        Log.Information(_seancePrompt);
    }

    public async Task<ChatCompletionCreateResponse> HaveASeance()
    {
        var messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem(_seancePrompt),
                ChatMessage.FromUser("Lets have a seance.")
            };
        var completionResult = await _aiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Model = Models.ChatGpt3_5Turbo,
            Messages = messages
            //MaxTokens = 50//optional
        });

        Log.Information(completionResult.Choices.First().Message.Content);
        messages.Add(completionResult.Choices.First().Message);
        await File.WriteAllTextAsync(_savePath, JsonSerializer.Serialize(messages));
        return completionResult;
    }

    public async Task RespondToSeanceAsync(DiscordClient dClient, ComponentInteractionCreateEventArgs args)
    {
        var userResponse = args.Interaction.Data.CustomId;
        string savedConvo = await File.ReadAllTextAsync(_savePath);
        var conversation = JsonSerializer.Deserialize<List<ChatMessage>>(savedConvo);
        conversation!.Add(ChatMessage.FromUser(userResponse));

        // COPY

        var completionResult = await _aiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Model = Models.ChatGpt3_5Turbo,
            Messages = conversation
        });
        conversation.Add(completionResult.Choices.First().Message);
        
        await File.AppendAllTextAsync(_savePath, JsonSerializer.Serialize(conversation));

        // END COPY

        DiscordButtonComponent[] buttons = await DiscordHelper.GetDiscordButtonsAsync(dClient);
        DiscordMessageBuilder builder = new();
        builder.AddEmbed(new DiscordEmbedBuilder()
            .WithTitle("Seance")
            .WithColor(DiscordColor.Grayple)
            .WithDescription(completionResult.Choices.First().Message.Content))
        .AddComponents(buttons);
       
        var message = await args.Channel.SendMessageAsync(builder);
        await File.WriteAllTextAsync(_savePath, JsonSerializer.Serialize(conversation));
    }
}

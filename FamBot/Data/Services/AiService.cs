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

namespace FamBot.Data.Services;
public class AiService
{
    private readonly string _savePath = "Data/Prompts/SeanceLog.json";
    private readonly string _promptPath = "Data/Prompts/SeancePrompt.txt";
    OpenAIService _aiService;
    string _seancePrompt;

    public AiService(IConfiguration config)
    {
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
        await File.WriteAllTextAsync(_savePath, JsonSerializer.Serialize(messages));
        return completionResult;
    }

    public async Task<ChatCompletionCreateResponse> RespondToSeanceAsync(string userResponse)
    {
        string savedConvo = await File.ReadAllTextAsync(_savePath);
        var conversation = JsonSerializer.Deserialize<List<ChatMessage>>(savedConvo);
        conversation!.Add(ChatMessage.FromUser(userResponse));
        var completionResult = await _aiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Model = Models.ChatGpt3_5Turbo,
            Messages = conversation
        });
        Log.Information($"{userResponse}");
        await File.AppendAllTextAsync("", JsonSerializer.Serialize(conversation));
        return completionResult;
    }
}

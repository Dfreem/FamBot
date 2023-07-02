using Microsoft.AspNetCore.Mvc;
using System.IO.MemoryMappedFiles;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using Microsoft.AspNetCore.SignalR.Protocol;
using OpenAI;
using Serilog;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;

namespace FamBot.Data.Services;
public class AiService
{
    OpenAIService _aiService;
    IConfiguration _configuration;
    string _seancePrompt;

    public AiService(IConfiguration config, IServiceProvider services)
    {
        _aiService = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = config["OpenAi:Key"]!
        });
        _configuration = config;
        var promptPath = Path.Combine(Path.GetPathRoot("~")!, "Data/Prompts/SeancePrompt.txt");
        _seancePrompt = File.ReadAllText(promptPath);
        Log.Information(_seancePrompt);
    }

    public async Task<string> HaveASeance()
    {
        var completionResult = await _aiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem(_seancePrompt),
                ChatMessage.FromUser("Lets have a seance.")
            },
            Model = Models.ChatGpt3_5Turbo,
            //MaxTokens = 50//optional
        });
        Log.Information(completionResult.Choices.First().Message.Content);
        return completionResult.Choices.First().Message.Content;
    }

    #region private helper methods



    #endregion
}

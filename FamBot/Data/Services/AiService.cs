using Microsoft.AspNetCore.Mvc;
using System.IO.MemoryMappedFiles;
using OpenAI.Managers;

namespace FamBot.Data.Services;

[Route("api/[controller]")]
[ApiController]
public class AiService : ControllerBase
{
	OpenAIService _aiService;
	IConfiguration _configuration;

	public AiService(IConfiguration config, OpenAIService ai)
	{
		_aiService = ai;
		_configuration = config;
	}

	// ======== IN - PROGRESS (left off here) ========
	//[HttpPost]
	//public async Task<string> HaveASeance()
	//{
	//	var seancePrompt = _configuration["SeancePrompt"] ?? "unable to find prompt in configuration";
	//	_aiService.ChatCompletion()

 //   }
}

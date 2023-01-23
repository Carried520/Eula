using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Eula.Services.BasicGameService;

public class GameService : IGameService
{
    private static readonly Random Random = new();
    private readonly ILogger<IGameService> _logger;
    private readonly IQuizService _quizService;


    public GameService(ILogger<IGameService> logger , IQuizService quizService)
    {
        _logger = logger;
        _quizService = quizService;
    }

    public string Roll(IEmote emote , int number = 6)
    {
        int roll = Random.Next(0, number);
        
        IEmote troll = Emote.Parse("<:pantsgrab:1066028043237343353>");

        string? rollString = roll is 0 ? troll.ToString() : roll.ToString(); 

        return $" {emote} You rolled {rollString}";
    }


    public string Flip(string choice)
    {
        var coinDict = new Dictionary<int, string> { [0] = "heads", [1] = "tails" };
        int flip = Random.Next(0, 1);

        return coinDict[flip] == choice ? $"You rolled {coinDict[flip]} and won" : $"You rolled {coinDict[flip]} and lost";
    }




    public async Task<QuizService.Quiz> HandleQuiz(ulong userId) => await  _quizService.HandleQuiz(userId);


    public ComponentBuilder RespondToAnswer(bool isSuccess, string customId, SocketMessageComponent component) =>
         _quizService.RespondToAnswer(isSuccess, customId, component);
}
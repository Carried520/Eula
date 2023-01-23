using Discord;
using Discord.WebSocket;

namespace Eula.Services.BasicGameService;

public interface IGameService
{
    public string Roll(IEmote emote , int number = 6);
    public string Flip(string choice);
    public Task<QuizService.Quiz> HandleQuiz(ulong userId);

    public ComponentBuilder RespondToAnswer(bool isSuccess, string customId, SocketMessageComponent component);
    
}
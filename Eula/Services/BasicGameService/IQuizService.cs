using Discord;
using Discord.WebSocket;

namespace Eula.Services.BasicGameService;

public interface IQuizService
{
    public Task<QuizService.Quiz> HandleQuiz(ulong userId);
    public ComponentBuilder RespondToAnswer(bool isSuccess, string customId, SocketMessageComponent component);
}
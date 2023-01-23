using System.Web;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Eula.Services.BasicGameService;

namespace Eula.Interactions.AppCommands;

public class BaseGame : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IGameService _gameService;

    public BaseGame(IGameService gameService) => _gameService = gameService;


    [SlashCommand("roll", "roll a dice")]
    public async Task Roll(int number = 6)
    {
        Emoji? emote = Emoji.Parse(":game_die:");
        string roll = _gameService.Roll(emote , number);

        await RespondAsync(roll);
    }


    [SlashCommand("flip", "flip a coin")]
    public async Task Flip(string choice) => await RespondAsync(_gameService.Flip(choice));

    [SlashCommand("quiz", "make quiz")]
    public async Task Quiz()
    {
        await DeferAsync();
        
        QuizService.Quiz quiz = await _gameService.HandleQuiz(Context.User.Id);
        
        await FollowupAsync(HttpUtility.HtmlDecode(quiz.Question) , components: quiz.Answers.Build());

    }


    [ComponentInteraction("quiz_*_*")]
    public async Task HandleButton(bool hasSucceeded , string customId)
    {
        await DeferAsync();
        ComponentBuilder response = _gameService.RespondToAnswer(hasSucceeded, customId,
            Context.Interaction as SocketMessageComponent ?? throw new InvalidOperationException());

        await Context.Interaction.ModifyOriginalResponseAsync(x =>
        {
            x.Components = response.Build();
        });
    }
    
    
    


}
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Web;
using Discord;
using Discord.WebSocket;
using Eula.ListExtensions;

namespace Eula.Services.BasicGameService;

public class QuizService : IQuizService
{
    private readonly HttpClient _client;

    public QuizService(HttpClient client)
    {
        _client = client;
        _client.BaseAddress = new Uri("https://opentdb.com/api.php?amount=1");
    }


    public async Task<Quiz> HandleQuiz(ulong userId)
    {
        Results quiz = (await FetchQuiz())[0];

        string?[] inCorrectAnswers = quiz.IncorrectAnswers.Select(HttpUtility.HtmlDecode).ToArray();
        string correctAnswer = HttpUtility.HtmlDecode(quiz.CorrectAnswer);

        
        List<IMessageComponent> list = quiz.IncorrectAnswers
            .Select(incorrectAnswer =>
            {
                string answer = HttpUtility.HtmlDecode(incorrectAnswer);
                return new ButtonBuilder(answer, $"quiz_false_{answer}", ButtonStyle.Secondary).Build();
            }).Cast<IMessageComponent>().ToList();


        
        list.Add(new ButtonBuilder(HttpUtility.HtmlDecode(quiz.CorrectAnswer) , $"quiz_true_{correctAnswer}" , ButtonStyle.Secondary).Build());

        list.Shuffle();
        
        var row = new ActionRowBuilder();
        row.WithComponents(list);
        ComponentBuilder? builder = new ComponentBuilder()
            .AddRow(row);

        return new Quiz(quiz.Question , builder , inCorrectAnswers , correctAnswer);
    }


    public ComponentBuilder RespondToAnswer(bool isSuccess , string customId , SocketMessageComponent component)
    {
        var builder = new ComponentBuilder();


        List<string> message = component.Message.Components.FirstOrDefault()?.Components.Select(x => x.CustomId).ToList() ?? throw new InvalidOperationException();

        string? correct = message.FirstOrDefault(x => x.Contains("true"))?.Replace("quiz_true_" , "");
        
        if(isSuccess)
            builder.WithButton(customId, customId, ButtonStyle.Success,
                disabled: true);

        else
            builder.WithButton(correct, correct, ButtonStyle.Success, disabled: true)
                .WithButton(customId, customId, ButtonStyle.Danger,
                    disabled: true);

        return builder;

    }
    
    
    
    private async Task<Results[]> FetchQuiz()
    {
        var request = await _client.GetFromJsonAsync<QuizData>("api.php?amount=1");
        
        return request?.ResultsArray ?? throw new InvalidOperationException();
    }
    
    
    
    
    
    public record Quiz(string Question, ComponentBuilder Answers , string?[]? IncorrectAnswers , string CorrectAnswer);
    
    
    private record QuizData
    {
        [JsonPropertyName("results")]
        public Results[] ResultsArray { get; init; } = null!;
    }

    private record Results
    {
        [JsonPropertyName("question")]
        public string Question { get; init; } = null!;
        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer { get; init; } = null!;
        [JsonPropertyName("incorrect_answers")]
        public string[] IncorrectAnswers { get; init; } = null!;
    }
}
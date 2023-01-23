using System.Globalization;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Eula.Database;
using Eula.DependencyInjectionExtensions;
using Eula.Services;
using Eula.Services.AppCommandService;
using Eula.Services.BaseCommandService;
using Eula.Services.BasicGameService;
using Eula.Services.BossService;
using Eula.Services.GuildMissionService;
using Eula.Services.LogService;
using Eula.Services.ReadyService;
using Fergun.Interactive;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eula;

public static class Program
{
    static Program()
    {
        var cultureName = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentCulture = cultureName;
        Thread.CurrentThread.CurrentUICulture = cultureName;
    }
    public static async Task Main() => await Bot.StartAsync();

    public static ServiceProvider ConfigureServices(DiscordSocketClient client)
    {
        IConfiguration config = Config.Configuration;
        IServiceCollection services = new ServiceCollection()
            .AddSingleton(client)
            .AddSingleton<ILogService, LogService>()
            .AddSingleton<IReadyService, ReadyService>()
            .AddSingleton<IBaseCommandService, BaseCommandService>()
            .AddSingleton<IAppCommandService, AppCommandService>()
            .AddSingleton(new CommandService(new CommandServiceConfig
                { DefaultRunMode = Discord.Commands.RunMode.Async }))
            .AddSingleton(new InteractionService(client,
                new InteractionServiceConfig
                    { DefaultRunMode = Discord.Interactions.RunMode.Async, AutoServiceScopes = true }))
            .AddSingleton<InteractiveService>()
            .AddSerilog()
            .AddSingleton<IEventListener, EventListener>()
            .AddSingleton<IBossService, BossService>()
            .AddTransient<IGameService, GameService>()
            .AddTransient<IQuizService,QuizService>()
            .AddTransient<IGuildMissionService, GuildMissionService>()
            .AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(config["ConnectionStrings:Postgres"]))
            .AddSingleton(config);

        services.AddHttpClient<IQuizService, QuizService>();


        return services.BuildServiceProvider();
    }

    internal static bool IsDebug
    {
        get
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}
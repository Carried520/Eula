using System.Globalization;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Eula.Database;
using Eula.DependencyInjectionExtensions;
using Eula.Services;
using Eula.Services.AppCommandService;
using Eula.Services.BaseCommandService;
using Eula.Services.GuildService;
using Eula.Services.LogService;
using Eula.Services.ReadyService;
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
            .AddSerilog()
            .AddSingleton<IEventListener,EventListener>()
            .AddTransient<IGuildMissionService, GuildMissionService>()
            .AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(config["ConnectionStrings:Postgres"]))
            .AddSingleton(config);


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
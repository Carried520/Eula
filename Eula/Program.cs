using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Eula.Database;
using Eula.DependencyInjectionExtensions;
using Eula.Services.AppCommandService;
using Eula.Services.BaseCommandService;
using Eula.Services.LogService;
using Eula.Services.ReadyService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using RunMode = Discord.Commands.RunMode;

namespace Eula;

public static class Program
{
    

    public static async Task Main()
    {
        var discord = new DiscordSocketClient(Config.DiscordConfig);
        IServiceProvider services = ConfigureServices(discord);
        var config = services.GetRequiredService<IConfiguration>();
        string? token = IsDebug ? config["Discord:Debug"] : config["Discord:Build"];

        await services.GetRequiredService<ILogService>().StartAsync();
        await services.GetRequiredService<IReadyService>().StartAsync();
        await services.GetRequiredService<IBaseCommandService>().StartAsync();
        await services.GetRequiredService<IAppCommandService>().StartAsync();


        await discord.LoginAsync(TokenType.Bot, token);
        await discord.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private static IServiceProvider ConfigureServices(DiscordSocketClient client)
    {
        IConfiguration config = Config.Configuration;
        IServiceCollection services = new ServiceCollection()
            .AddSingleton(client)
            .AddSingleton<ILogService, LogService>()
            .AddSingleton<IReadyService, ReadyService>()
            .AddSingleton<IBaseCommandService, BaseCommandService>()
            .AddSingleton<IAppCommandService, AppCommandService>()
            .AddSingleton(new CommandService(new CommandServiceConfig { DefaultRunMode = RunMode.Async }))
            .AddSingleton(new InteractionService(client,
                new InteractionServiceConfig
                    { DefaultRunMode = Discord.Interactions.RunMode.Async, AutoServiceScopes = true }))
            .AddSerilog()
            .AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(config["ConnectionStrings:Postgres"]))
            .AddSingleton(config);


        return services.BuildServiceProvider(true);
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
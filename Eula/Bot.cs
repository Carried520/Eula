using System.Globalization;
using Discord;
using Discord.WebSocket;
using Eula.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eula;

public static class Bot
{
    public static async Task StartAsync()
    {
        
        var discord = new DiscordSocketClient(Config.DiscordConfig);
        await using ServiceProvider services = Program.ConfigureServices(discord);

        var config = services.GetRequiredService<IConfiguration>();
        string? token = Program.IsDebug ? config["Discord:Debug"] : config["Discord:Build"];

        await services.GetRequiredService<IEventListener>().StartAsync();
        
        await discord.LoginAsync(TokenType.Bot, token);
        await discord.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }
    
}
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace Eula;

public static class Config
{
    public static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
    }

    public static DiscordSocketConfig DiscordConfig { get; set; } = new()
    {
        AlwaysDownloadUsers = true,
        LogLevel = LogSeverity.Debug,
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers | GatewayIntents.MessageContent,
        TotalShards = 1

    };
    
    
}
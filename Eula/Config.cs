using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace Eula;

public static class Config
{
    static Config()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        DiscordConfig = new DiscordSocketConfig
        {
            AlwaysDownloadUsers = true,
            LogLevel = LogSeverity.Debug,
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers |
                             GatewayIntents.MessageContent,
            TotalShards = 1
        };
    }

    public static IConfiguration Configuration { get; }


    public static DiscordSocketConfig DiscordConfig { get; }
}
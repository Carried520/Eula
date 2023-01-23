using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Eula.Services.ReadyService.BossTimer;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Eula.Services.BossService;

public class BossService : IBossService
{
    private readonly DiscordSocketClient _discord;
    private readonly ILogger<IBossService> _logger;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(1));

    public BossService(DiscordSocketClient discord , ILogger<IBossService> logger)
    {
        _discord = discord;
        _logger = logger;
    }

    private static ulong GuildId => Program.IsDebug ? 569505274667466762UL : 875583069678092329UL;
    private static ulong ChannelId => Program.IsDebug ? 699396897994965061UL : 875585800870457355UL;




    public async Task StartAsync() => await Send();
    
    private async Task Send()
    {
        if (Program.IsDebug) return;
        SocketGuild? guild = _discord.GetGuild(GuildId);
        SocketTextChannel? channel = guild.GetTextChannel(ChannelId);

        try
        {
            await channel.DeleteMessagesAsync(await channel.GetMessagesAsync(1).FlattenAsync());
        }
        catch (Exception e)
        {
            _logger.LogInformation("Unexpected boss timer exception : \n {}" , e);
        }
        
        List<string> bosses = await BossSchedule.Bosses();
        var embed = new EmbedBuilder { Description = $"{bosses.First()}" };
        RestUserMessage? message = await channel.SendMessageAsync(embed: embed.Build());
            

        while (await _timer.WaitForNextTickAsync())
            await DoTimerAsync(message);


    }
    
    
    
    private static async Task DoTimerAsync(RestUserMessage? message)
    {
        List<string> bosses = await BossSchedule.Bosses();
        if (bosses.First() != message?.Embeds.First().Description)
        {
            var embedded = new EmbedBuilder { Description = $"{bosses.First()}" };
            await message?.ModifyAsync(x => x.Embed = embedded.Build())!;
        }

    }

    
}
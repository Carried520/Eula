using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Eula.Services.BossService;
using Microsoft.Extensions.Logging;

namespace Eula.Services.ReadyService;

public class ReadyService : IReadyService
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interaction;
    private readonly IBossService _bossService;
    private readonly ILogger<IReadyService> _log;


    public ReadyService(DiscordSocketClient client, ILogger<IReadyService> log, InteractionService interaction , IBossService bossService)
    {
        _client = client;
        _log = log;
        _interaction = interaction;
        _bossService = bossService;
    }

    public async Task StartAsync()
    {
        _client.Ready += Ready;

        await Task.CompletedTask;
    }

    private  Task Ready()
    {

        
        _ = Task.Run(async () => await Task.WhenAll(RegisterSlashCommandsAsync() , HandleBossStart()));

        return Task.CompletedTask;
    }



    private async Task RegisterSlashCommandsAsync()
    {

        try
        {

            _log.LogInformation("Logged in as {username}#{discriminator}", _client.CurrentUser.Username,
                _client.CurrentUser.Discriminator);
            if (Program.IsDebug)
            {
                IReadOnlyCollection<RestGuildCommand>? commands =
                    await _interaction.RegisterCommandsToGuildAsync(569505274667466762UL);
                _log.LogInformation("Registered Commands : {@list}", commands.Select(x => x.Name));
            }

            else
            {
                await _interaction.RegisterCommandsGloballyAsync();
            }
        }
        catch (Exception e)
        {
            await HandleException(e);
        }



    }

    private async Task HandleBossStart()
    {
        try
        {
            await _bossService.StartAsync();
        }
        catch (Exception e)
        {
            await HandleException(e);
        }
    }
    
    
    
    private async Task HandleException(Exception e)
    {
        
        _log.LogError("[Exception] {e}", e);
        await Task.CompletedTask;
        
    }
}
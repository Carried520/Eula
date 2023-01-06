using Discord.Interactions;
using Discord.WebSocket;
using Eula.Services.LogService;

namespace Eula.Services.ReadyService;

public class ReadyService : IReadyService
{

    private readonly DiscordSocketClient _client;
    private readonly ILogService _log;
    private readonly InteractionService _interaction;


    public ReadyService(DiscordSocketClient client , ILogService log , InteractionService interaction)
    {
        _client = client;
        _log = log;
        _interaction = interaction;
    }

    public async Task StartAsync()
    {
        _client.Ready += Ready;
        
        await Task.CompletedTask;
    }

    private async Task Ready()
    {
        async Task Func(Exception e)
        {
            _log.GetLogger.Error("[Exception] {e}", e);
            await Task.CompletedTask;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                _log.GetLogger.Information("Logged in as {username}#{discriminator}", _client.CurrentUser.Username,
                    _client.CurrentUser.Discriminator);
                if (Program.IsDebug)
                {
                    var commands = await _interaction.RegisterCommandsToGuildAsync(569505274667466762UL);
                    _log.GetLogger.Information("Registered Commands : {@list}", commands.Select(x => x.Name));
                }

                else
                    await _interaction.RegisterCommandsGloballyAsync();
            }
            catch (Exception e)
            {
               await Func(e);
            }

            await Task.CompletedTask;





        });
        
        await Task.CompletedTask;
    }

}
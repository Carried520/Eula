using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Eula.Services.LogService;

namespace Eula.Services.AppCommandService;

public class AppCommandService : IAppCommandService
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _commands;
    private readonly ILogService _log;
    private readonly IServiceProvider _services;

    public AppCommandService(ILogService log, DiscordSocketClient client, InteractionService commands, IServiceProvider services)
    {
        _log = log;
        _client = client;
        _commands = commands;
        _services = services;
    }


    public async Task StartAsync()
    {
        
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        _client.InteractionCreated += RespondToInteraction;
        _commands.InteractionExecuted += InteractionExecuted;
    }

    private async Task RespondToInteraction(SocketInteraction e)
    {
        var ctx = new SocketInteractionContext(_client , e);
        await _commands.ExecuteCommandAsync(ctx, _services);

    }

    private  async Task InteractionExecuted(ICommandInfo info, IInteractionContext context, IResult result)
    {
        if(result.IsSuccess)
            return;

        var response = result.Error switch
        {
            InteractionCommandError.UnmetPrecondition => $"Unmet Precondition : \n {result.ErrorReason}",
            InteractionCommandError.UnknownCommand => "Unknown Command",
            InteractionCommandError.BadArgs => "Invalid arguments",
            InteractionCommandError.Exception => $"Command Exception :\n {result.ErrorReason}",
            InteractionCommandError.Unsuccessful => "Command could not be executed",
            _ => result.ErrorReason
        };

        _log.GetLogger.Warning("{error}" , response);
        await context.Interaction.RespondAsync(response);
    }


}
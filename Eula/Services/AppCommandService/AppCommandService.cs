using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Eula.Services.LogService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Eula.Services.AppCommandService;

public class AppCommandService : IAppCommandService
{
    private readonly ILogger<IAppCommandService> _logger;
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _commands;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceProvider _services;

    public AppCommandService(ILogger<IAppCommandService> logger, DiscordSocketClient client, InteractionService commands,
        IServiceProvider services, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _client = client;
        _commands = commands;
        _services = services;
        _scopeFactory = scopeFactory;
    }


    public async Task StartAsync()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), scope.ServiceProvider);
        _client.InteractionCreated += RespondToInteraction;
        _commands.InteractionExecuted += InteractionExecuted;
    }

    private async Task RespondToInteraction(SocketInteraction e)
    {
        var ctx = new SocketInteractionContext(_client, e);
        await _commands.ExecuteCommandAsync(ctx, _services);
    }

    private async Task InteractionExecuted(ICommandInfo info, IInteractionContext context, IResult result)
    {
        if (result.IsSuccess)
            return;

        string? response = result.Error switch
        {
            InteractionCommandError.UnmetPrecondition => $"Unmet Precondition : \n {result.ErrorReason}",
            InteractionCommandError.UnknownCommand => "Unknown Command",
            InteractionCommandError.BadArgs => "Invalid arguments",
            InteractionCommandError.Exception => $"Command Exception :\n {result.ErrorReason}",
            InteractionCommandError.Unsuccessful => "Command could not be executed",
            _ => result.ErrorReason
        };

        _logger.LogError("{error}", response);
        await context.Interaction.RespondAsync(response);
    }
}
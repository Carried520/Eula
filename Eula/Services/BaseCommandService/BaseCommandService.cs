using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Eula.Services.LogService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Eula.Services.BaseCommandService;

public class BaseCommandService : IBaseCommandService
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commandService;
    private readonly IConfiguration _config;
    private readonly ILogger<IBaseCommandService> _log;
    private readonly IServiceProvider _services;

    public BaseCommandService(DiscordSocketClient client, CommandService commandService, IServiceProvider services,
        IConfiguration config, ILogger<IBaseCommandService> log)
    {
        _client = client;
        _commandService = commandService;
        _services = services;
        _config = config;
        _log = log;
    }

    public async Task StartAsync()
    {
        _client.MessageReceived += HandleCommandAsync;
        _commandService.CommandExecuted += CommandExecutedAsync;

        await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }

    private async Task HandleCommandAsync(SocketMessage message)
    {
        if (message is not SocketUserMessage { Source: MessageSource.User } msg) return;
        var argPos = 0;

        string? generalPrefix = Program.IsDebug ? _config["Prefix:Debug"] : _config["Prefix:Build"];
        bool isGeneralPrefixPresent =
            msg.HasStringPrefix(generalPrefix, ref argPos, StringComparison.OrdinalIgnoreCase);

        if (!isGeneralPrefixPresent)
            return;
        var context = new SocketCommandContext(_client, msg);
        await _commandService.ExecuteAsync(context, argPos, _services);
    }


    private async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
    {
        if (!command.IsSpecified) return;

        if (!result.IsSuccess)
        {
            _log.LogError("[Command]{exception}", result.Error);
            await context.Channel.SendMessageAsync($"error: {result}");
        }
    }
}
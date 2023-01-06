using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Eula.Services.LogService;
using Microsoft.Extensions.Configuration;

namespace Eula.Services.BaseCommandService;

public class BaseCommandService : IBaseCommandService
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commandService;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _config;
    private readonly ILogService _log;

    public BaseCommandService(DiscordSocketClient client , CommandService commandService , IServiceProvider services , IConfiguration config , ILogService log)
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

        await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), services: _services);
    }

    private async Task HandleCommandAsync(SocketMessage message)
    {
        if (message is not SocketUserMessage  { Source: MessageSource.User } msg) return;
        var argPos = 0;
    
        var generalPrefix = Program.IsDebug ? _config["Prefix:Debug"] : _config["Prefix:Build"];
        var isGeneralPrefixPresent = msg.HasStringPrefix(generalPrefix, ref argPos, StringComparison.OrdinalIgnoreCase);
        
        if (!isGeneralPrefixPresent)
            return;
        var context = new SocketCommandContext(_client, msg);
        await _commandService.ExecuteAsync(context, argPos, _services);

    }
    
    
    private async  Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
    {
        if (!command.IsSpecified)
        {
            return;
        }

        if (!result.IsSuccess)
        {
            _log.GetLogger.Error("[Command]{exception}" , result.Error);
            await context.Channel.SendMessageAsync($"error: {result}");
        }
        
    }
    
    
}
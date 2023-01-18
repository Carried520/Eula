using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Eula.Services.LogService;

public class LogService : ILogService
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _command;
    private readonly ILogger<ILogService> _logger;

    public LogService(DiscordSocketClient client, CommandService command, ILogger<ILogService> logger)
    {
        _client = client;
        _command = command;
        _logger = logger;
    }

    public async Task StartAsync()
    {
        _client.Log += LogDiscordEvents;
        _command.Log += LogDiscordEvents;
        await Task.CompletedTask;
    }
    


    private  async Task LogDiscordEvents(LogMessage message)
    {
        LogLevel severity = message.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.Information
        };
        
        _logger.Log(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
        await Task.CompletedTask;
    }
}
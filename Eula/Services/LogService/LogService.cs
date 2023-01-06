using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Serilog;
using Serilog.Core;
using Serilog.Events;


namespace Eula.Services.LogService;

public class LogService : ILogService
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _command;

    public LogService(DiscordSocketClient client, CommandService command)
    {
        _client = client;
        _command = command;
        Log.Logger = Log.Logger = new LoggerConfiguration()
            .WriteTo.Async(a => a.Console())
            .Enrich.FromLogContext()
            .CreateLogger();

    }

    public async Task StartAsync()
    {
        _client.Log += LogDiscordEvents;
        _command.Log += LogDiscordEvents;
        await Task.CompletedTask;
    }

    public ILogger GetLogger => Log.Logger;


    private async Task LogDiscordEvents(LogMessage message)
    {
        var severity = message.Severity switch
        {
            LogSeverity.Critical => LogEventLevel.Fatal,
            LogSeverity.Error => LogEventLevel.Error,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Info => LogEventLevel.Information,
            LogSeverity.Verbose => LogEventLevel.Verbose,
            LogSeverity.Debug => LogEventLevel.Debug,
            _ => LogEventLevel.Information
        };
        Log.Write(severity, message.Exception, "[{Source}] {Message} ", message.Source, message.Message);
        await Task.CompletedTask;
    }
    
    
}
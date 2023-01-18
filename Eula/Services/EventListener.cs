using System.Globalization;
using Eula.Services.AppCommandService;
using Eula.Services.BaseCommandService;
using Eula.Services.LogService;
using Eula.Services.ReadyService;

namespace Eula.Services;

public class EventListener : IEventListener
{
    private readonly ILogService _log;
    private readonly IBaseCommandService _baseCommandService;
    private readonly IAppCommandService _appCommandService;
    private readonly IReadyService _readyService;

    public EventListener(ILogService log , IBaseCommandService baseCommandService , IAppCommandService appCommandService , IReadyService readyService)
    {
        _log = log;
        _baseCommandService = baseCommandService;
        _appCommandService = appCommandService;
        _readyService = readyService;
    }
    
    public async Task StartAsync()
    {
        
        await _log.StartAsync();
        await _baseCommandService.StartAsync();
        await _appCommandService.StartAsync();
        await _readyService.StartAsync();
    }
}
using Serilog;

namespace Eula.Services.LogService;

public interface ILogService
{
    public Task StartAsync();
}
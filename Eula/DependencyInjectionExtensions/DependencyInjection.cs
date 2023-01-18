using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;

namespace Eula.DependencyInjectionExtensions;

public static class DependencyInjection
{
    public static IServiceCollection AddSerilog(this IServiceCollection collection)
    {
        Logger logger =  new LoggerConfiguration()
            .WriteTo.Async(a => a.Console(theme: AnsiConsoleTheme.Literate))
            .Enrich.FromLogContext()
            .CreateLogger();

       return collection.AddLogging(x => x.AddSerilog(logger));
    }
}   
using System.Globalization;
using Eula.Services.ReadyService.BossTimer;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Text;

namespace Eula.Services.BossService;

public  class BossTimer
{
    
    
    /// <summary>
    ///     Sets List of Bosses
    /// </summary>
    /// <returns>List of Bosses</returns>
    public static async Task<List<BossData>> GetBosses()
    {
        DateTimeZone warsaw = DateTimeZoneProviders.Tzdb["Europe/Warsaw"];
        LocalDate nowDate = SystemClock.Instance.InZone(warsaw).GetCurrentDate();

        var pattern = LocalTimePattern.Create("H:mm", new CultureInfo("pl-PL"));
        IsoDayOfWeek weekday = nowDate.DayOfWeek;
        LocalDateTime localNow = SystemClock.Instance.InZone(warsaw).GetCurrentLocalDateTime();
        LocalDateTime comparisonNow = localNow.Date + pattern.Parse("23:15").Value;
        Dictionary<string, IEnumerable<BossDataTemp>> parser = await BossParser.Parse();
        IEnumerable<BossDataTemp> listOfBosses;

        if (weekday is IsoDayOfWeek.Sunday && localNow > comparisonNow)
            listOfBosses = parser["Sunday"];
        else if (localNow > comparisonNow)
        {
            string? name = Enum.GetName(typeof(IsoDayOfWeek), (int)weekday + 1);
            listOfBosses = parser[name ?? throw new InvalidOperationException()];
        }
        else
        {
            string? name = Enum.GetName(typeof(IsoDayOfWeek), (int)weekday);
            listOfBosses = parser[name ?? throw new InvalidOperationException()];
        }


        return listOfBosses.Select(item => new BossData
            { Boss = item.Boss, BossSpawnTime = localNow.Date + pattern.Parse(item.SpawnTime).Value }).ToList();
    }

   
}

/// <summary>
///     Simple Data Class Representing Bosses
/// </summary>
public record BossData
{
    public string? Boss { get; init; }
    public LocalDateTime BossSpawnTime { get; init; }
}
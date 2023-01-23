using System.Globalization;
using Discord;
using Eula.Services.BossService;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Text;
using Serilog;

namespace Eula.Services.ReadyService.BossTimer;

public static class BossSchedule
{
    /// <summary>
    ///     Retrieves List of Bosses to spawn
    /// </summary>
    /// <returns>List of Bosses</returns>
    public static async Task<List<string>> Bosses()
    {
        var warn = "";
        string BossInfo;
        var finalArray = new List<string>();
        var boss = (await ReturnBosses()).OrderBy(x => x.BossSpawnTime).ToList();

        if (boss.Any())
        {
            BossData FirstElement = boss.First();
            DateTime spawn = FirstElement.BossSpawnTime.ToDateTimeUnspecified();
            BossInfo = $"{FirstElement.Boss} \n {TimestampTag.FromDateTime(spawn, TimestampTagStyles.Relative)} " +
                       $"\n Spawns on : \n {TimestampTag.FromDateTime(spawn)}";
            finalArray.Add(BossInfo);
            DateTimeZone Warsaw = DateTimeZoneProviders.Tzdb["Europe/Warsaw"];
            LocalDateTime now = SystemClock.Instance.InZone(Warsaw).GetCurrentLocalDateTime();
            Period diff = now - FirstElement.BossSpawnTime;
            if (diff.Minutes < 11)
                warn = $"{diff.Minutes}";
        }
        else
        {
            BossInfo = "No boss today anymore";
            finalArray.Add(BossInfo);
        }

        finalArray.Add(warn);

        return finalArray;
    }

    private async static Task<List<BossData>> ReturnBosses()
    {
        List<BossData> bossTime = await BossService.BossTimer.GetBosses();

        DateTimeZone warsaw = DateTimeZoneProviders.Tzdb["Europe/Warsaw"];
        LocalDateTime now = SystemClock.Instance.InZone(warsaw).GetCurrentLocalDateTime();
        LocalDate nowDate = SystemClock.Instance.InZone(warsaw).GetCurrentDate();
        var pattern = LocalTimePattern.Create("H:mm", new CultureInfo("pl-PL"));

        var data = new List<BossData>();
        foreach (BossData bossItem in bossTime)
        {
            Log.Information($"Getting {bossItem.Boss}");
            LocalDateTime bossSpawn = bossItem.BossSpawnTime;
            HandleSingleItemAsync(now, nowDate, pattern, bossSpawn, data, bossItem);
        }

        return data;
    }

    private static void HandleSingleItemAsync(LocalDateTime now, LocalDate nowDate, LocalTimePattern pattern,
        LocalDateTime bossSpawn, ICollection<BossData> data, BossData bossItem)
    {
        if (now > nowDate + pattern.Parse("23:15").Value)
        {
            LocalDateTime nextDay = now.PlusDays(1);
            var tomorrow = new LocalDateTime(nextDay.Year, nextDay.Month, nextDay.Day, bossSpawn.Hour,
                bossSpawn.Minute, bossSpawn.Second);
            Period diff = tomorrow - now;
            if (diff.Milliseconds <= 0) return;
            data.Add(bossItem with { BossSpawnTime = tomorrow });
        }
        else
        {
            Period diff = bossSpawn - now;
            if (diff.Milliseconds <= 0) return;
            data.Add(bossItem);
        }
    }
}
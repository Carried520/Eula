using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Eula.Services.BossService;

public static class BossParser
{
    public static async Task<Dictionary<string, IEnumerable<BossDataTemp>>> Parse()
    {
        await using FileStream fs = File.OpenRead("Resources/BossList.json");
        var root = await JsonSerializer.DeserializeAsync<Dictionary<string, IEnumerable<BossDataTemp>>>(fs);
        return root ?? throw new JsonException("Invalid json ");
    }
}

public record BossDataTemp
{
    [JsonPropertyName("boss")]
    [UsedImplicitly]
    public required string Boss { get; init; }

    [JsonPropertyName("spawntime")]
    [UsedImplicitly]
    public required string SpawnTime { get; init; }
}
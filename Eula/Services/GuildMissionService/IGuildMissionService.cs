using System.Text;
using Eula.Interactions.AppCommands.Guild;

namespace Eula.Services.GuildMissionService;

public interface IGuildMissionService
{
    public Task<List<GuildMissionService.PlayerData>> ExecuteMission(GuildMissionType guildMissionType,
        IEnumerable<string> players);

    public Task<List<GuildMissionService.PlayerData>> GetAllPlayersAsync();

    public Task<(StringBuilder family, StringBuilder points, StringBuilder tier)> BuildString();

    public Task<int> DeleteAllEntriesAsync();
}
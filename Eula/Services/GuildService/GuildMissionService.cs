using System.Globalization;
using System.Text;
using Dawn;
using Eula.Database;
using Eula.Database.Models;
using Eula.Interactions.AppCommands.Guild;
using Microsoft.EntityFrameworkCore;

namespace Eula.Services.GuildService;

public class GuildMissionService : IGuildMissionService
{

    private readonly ApplicationDbContext _dbContext;

    public GuildMissionService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }




    public async Task<(StringBuilder family , StringBuilder points , StringBuilder tier)> BuildString()
    {
        List<PlayerData> players = await GetAllPlayersAsync();
        
        var familyBuilder = new StringBuilder();
        var pointBuilder = new StringBuilder();
        var tierBuilder = new StringBuilder();
        
        players.ForEach(x =>
        {
            familyBuilder.AppendLine(x.Player);
            pointBuilder.AppendLine(x.Points.ToString(CultureInfo.InvariantCulture));
            tierBuilder.AppendLine(x.Tier.ToString(CultureInfo.InvariantCulture));
        });


        return (familyBuilder, pointBuilder, tierBuilder);

    }


    public async Task<List<PlayerData>> GetAllPlayersAsync()
    {
        List<PlayerData> players =
            await _dbContext.GuildMissionDatas.Select(
                x => new PlayerData(x.UserName, x.Points, x.Points, x.Points)).ToListAsync();


        Guard.Argument(players).NotEmpty((input) =>"Didnt find any player data");
        return players;
        

    }   


    public async Task<int> DeleteAllEntriesAsync()
    {
        int deletedEntries = await _dbContext.GuildMissionDatas.ExecuteDeleteAsync();
        return deletedEntries;
    }


    public async Task<List<PlayerData>> ExecuteMission(GuildMissionType guildMissionType , IEnumerable<string> players)
    {
        List<string> list = players.ToList();

        List<PlayerData> listOfPlayerData = new();


        foreach (string player in list)
        {
            listOfPlayerData.Add(await AddOrUpdateAsync(CalculatePoints(guildMissionType) , player));
        }

        return listOfPlayerData;
    }

    private async Task<GuildMissionEntity?> GetPlayerAsync(string player) => await _dbContext.GuildMissionDatas.FirstOrDefaultAsync(x => x.UserName == player);

    private async Task<PlayerData> AddOrUpdateAsync(double pointGain, string player)
    {


        GuildMissionEntity? query = await GetPlayerAsync(player);
        double previousPoints;
        if (query is not null)
        {
            previousPoints = query.Points;
            query.Points += pointGain;
        }
           
        else
        {
            query = new GuildMissionEntity { Points = pointGain, UserName = player };
            previousPoints = 0;
            _dbContext.Add(query);
        }

        await _dbContext.SaveChangesAsync();
        return new PlayerData(player , previousPoints ,query.Points , query.Points);
    }

    private double CalculatePoints(GuildMissionType type) =>
        type switch
        {
            GuildMissionType.Boss => 2,
            GuildMissionType.Regular => 1,
            GuildMissionType.Smh => 0.5,
            _ => throw new ArgumentNullException(nameof(type))
        };

    public record PlayerData(string Player , double PreviousPoints ,  double Points , double Tier);
}
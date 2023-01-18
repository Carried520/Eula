using System.Globalization;
using System.Text;
using Discord;
using Discord.Interactions;
using Eula.Services.GuildService;
using Eula.Validators;
using FluentValidation;
using JetBrains.Annotations;

namespace Eula.Interactions.AppCommands.Guild;

public class GuildMission : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IGuildMissionService _guildMissionService;

    public GuildMission(IGuildMissionService guildMissionService) => _guildMissionService = guildMissionService;

    [SlashCommand("guildmission" , "execute guildmission command replace here")]
    public async Task GuildMissionAsync([ComplexParameter] GuildMissionData data)
    {
        await DeferAsync();
        var validator = new GuildMissionValidator();
       
        
        // ReSharper disable once MethodHasAsyncOverload
        validator.ValidateAndThrow(data);
         List<GuildMissionService.PlayerData> outcome = await _guildMissionService.ExecuteMission(data.MissionType , data.Players.ToArray());

        StringBuilder resultString = new StringBuilder().AppendLine("```");
        
        outcome.ForEach(x => resultString.AppendLine($"{x.Player} {x.PreviousPoints} -> {x.Points}"));
        await FollowupAsync(resultString.AppendLine("```").ToString());
    }
    
    
    [SlashCommand("listmissions" , "list guild missions")]
    public async Task ListMissions()
    {
        await DeferAsync();

        (StringBuilder family, StringBuilder points, StringBuilder tier) getStrings = await  _guildMissionService.BuildString();

        Embed? embed = new EmbedBuilder { Color = Color.DarkBlue }
            .AddField("Family", getStrings.family.ToString(), true)
            .AddField("Points", getStrings.points.ToString(), true)
            .AddField("Tier", getStrings.tier.ToString(), true)
            .Build();
        
        await FollowupAsync(embed: embed);
    }



    [SlashCommand("deleteallmissions", "delete all missions")]
    public async Task DeleteMissions()
    {
        await DeferAsync();

        int deletedEntries = await _guildMissionService.DeleteAllEntriesAsync();


        await FollowupAsync($"Deleted {deletedEntries} Entries ");
    }
    
    
    
}





[UsedImplicitly]
public class GuildMissionData
{
    
    [ComplexParameterCtor]
    public GuildMissionData(string[] players, GuildMissionType missionType)
    {
        Players = players;
        MissionType = missionType;
    }

    public string[] Players { get;}
    public GuildMissionType MissionType { get; }
    
}

public enum GuildMissionType
{
    Smh,
    Regular,
    Boss
}
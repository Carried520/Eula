using Discord;
using Discord.Interactions;

namespace Eula.Converters;

public class StringArrayConverter : TypeConverter<string[]>
{
    public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.String;
    
    public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
    {
        string[]? stringArray = (option.Value as string)?.Split(",");
        return Task.FromResult(TypeConverterResult.FromSuccess(stringArray));
    }
}
using Eula.Interactions.AppCommands.Guild;
using FluentValidation;

namespace Eula.Validators;

public class GuildMissionValidator : AbstractValidator<GuildMissionData>
{

    private const int MinLength = 5;
    private const int MaxLength = 100;
    public GuildMissionValidator()
    {
        RuleForEach(data => data.Players)
            .OverrideIndexer((_, _, _, arg4) => $" {arg4 + 1} ")
            .Length(MinLength, MaxLength)
            .WithName("Player");

    }
}
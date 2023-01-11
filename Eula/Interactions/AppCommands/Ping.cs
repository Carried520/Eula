using Discord.Interactions;

namespace Eula.Interactions.AppCommands;

public class Ping : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "send pong")]
    public async Task ExecuteAsync()
    {
        await RespondAsync("pong");
    }
}
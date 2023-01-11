using Discord.Interactions;

namespace Eula.Interactions.AppCommands;

public class AnotherTest : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("anothertest", "send pong")]
    public async Task ExecuteAsync()
    {
        await RespondAsync("pong");
    }
}
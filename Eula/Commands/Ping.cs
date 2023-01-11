using Discord.Commands;

namespace Eula.Commands;

public class Ping : ModuleBase<SocketCommandContext>
{
    [Command("ping")]
    public async Task ExecuteAsync()
    {
        await ReplyAsync("pong");
    }
}
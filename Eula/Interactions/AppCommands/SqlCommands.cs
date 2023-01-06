using Discord;
using Discord.Interactions;
using Eula.Database;
using Microsoft.EntityFrameworkCore;

namespace Eula.Interactions.AppCommands;

public class SqlCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ApplicationDbContext _context;

    public SqlCommands(ApplicationDbContext context) => _context = context;

    [SlashCommand("insert", "insert")]
    public async Task InsertOrUpdate()
    {
        var query = await _context.Users.FirstOrDefaultAsync(x => x.Id == Context.User.Id);
        if (query != null)
            query.Name = Context.User.Username;
        else
        {
            var instance = new UserEntity { Id = Context.User.Id, Name = Context.User.Username };
            _context.Add(instance);
        }

        var entries = await _context.SaveChangesAsync();
        await ReplyAsync($"Inserted/Updated {entries} entries ");
    }


    [SlashCommand("listentries", "list of entries")]
    public async Task List()
    {
        var query = await _context.Users.ToListAsync();
        var embed = new EmbedBuilder();
        
        query.ForEach(x =>
        {
            embed.AddField("name", x.Name, true);
            embed.AddField("id", x.Id, true);
        });

        await ReplyAsync(embed: embed.Build());
    }
}
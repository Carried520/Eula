using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Eula.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Eula.Database;

public class ApplicationDbContext : DbContext
{

    public ApplicationDbContext()
    {
        
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<GuildMissionEntity> GuildMissionDatas { get; set; } = null!;


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Config.Configuration["ConnectionStrings:Postgres"])
            .LogTo(Console.WriteLine);
    }
}

public class UserEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Key { get; set; }

    public ulong Id { get; set; }
    public int Warns { get; set; }
}
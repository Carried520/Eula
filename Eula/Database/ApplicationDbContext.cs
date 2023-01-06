using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Eula.Database;

public class ApplicationDbContext : DbContext
{

    public ApplicationDbContext()
    {
        
    }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Config.CreateConfiguration()["ConnectionStrings:Postgres"]);
        
    }

    public DbSet<UserEntity> Users { get; set; } = null!;
}


public class UserEntity
{
    public ulong Id { get; set; }
    public required string Name { get; set; }
}
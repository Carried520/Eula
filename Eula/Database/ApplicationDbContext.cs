using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Eula.Services.LogService;
using Microsoft.EntityFrameworkCore;

namespace Eula.Database;

public class ApplicationDbContext : DbContext
{
    private readonly ILogService _logService;

    public ApplicationDbContext(ILogService logService)
    {
        _logService = logService;
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogService logService) : base(options)
    {
        _logService = logService;
    }

    public DbSet<UserEntity> Users { get; set; } = null!;


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
    public required string Name { get; set; }
}
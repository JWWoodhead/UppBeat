using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Uppbeat.Api.Data;

public class UppbeatDbContext : IdentityDbContext<UppbeatUser>
{
    public UppbeatDbContext(DbContextOptions<UppbeatDbContext> options)
    : base(options)
    {
    }

    public DbSet<Artist> Artists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        CreateIndexes(modelBuilder);
    }

    private static void CreateIndexes(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Artist>()
            .HasIndex(a => a.Name);
    }
}

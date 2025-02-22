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
    public DbSet<Track> Tracks { get; set; }
    public DbSet<TrackGenre> TrackGenres { get; set; }
    public DbSet<Genre> Genres { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        CreateIndexes(modelBuilder);

        // Define Many-to-Many relationship between tracks and genres. Cannot be properly defined via model attributes alone
        // Should also really be populated for other data models to avoid inconsistency
        modelBuilder
            .Entity<TrackGenre>()
            .HasOne(tg => tg.Track)
            .WithMany(t => t.TrackGenres)
            .HasForeignKey(tg => tg.TrackId);

        modelBuilder
            .Entity<TrackGenre>()
            .HasOne(tg => tg.Genre)
            .WithMany(g => g.TrackGenres)
            .HasForeignKey(tg => tg.GenreId);

        // Add some base genres you can associate tracks with until this can be managed via the API without manual DB intervention
        // Alternatively this could work on a more tag based approach and forgo a link table entirely (probably a better approach but only working based on what I know) 
        modelBuilder.Entity<Genre>().HasData(new List<Genre>()
        {
            new Genre { Id = 1, Name = "Funk" },
            new Genre { Id = 2, Name = "Techno" },
            new Genre { Id = 3, Name = "Reggae" },
        });
    }

    private static void CreateIndexes(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Artist>()
            .HasIndex(a => a.Name);

        modelBuilder
            .Entity<Track>()
            .HasIndex(t => t.ArtistId);

        modelBuilder
            .Entity<Track>()
            .HasIndex(t => new { t.Name, t.Id });

        modelBuilder
            .Entity<Genre>()
            .HasIndex(g => g.Name)
            .IsUnique();
    }
}

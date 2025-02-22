using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Uppbeat.Api.Data;

namespace Uppbeat.Api.IntegrationTests;

public class AnonymousWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    public UppbeatDbContext GetDbContext()
    {
        var scope = Services.CreateScope();

        return scope.ServiceProvider.GetRequiredService<UppbeatDbContext>();
    }

    internal Artist CreateArtist(Artist artist)
    {
        var context = GetDbContext();

        context.Artists.Add(artist);
        context.SaveChanges();

        return artist;
    }

    internal Genre CreateGenre(Genre genre)
    {
        var context = GetDbContext();

        context.Genres.Add(genre);
        context.SaveChanges();

        return genre;
    }

    public Track CreateTrackWithGenres(string name, Artist artist, int duration, string file, string[] genreNames)
    {
        var context = GetDbContext();

        var track = new Track
        {
            Name = name,
            ArtistId = artist.Id,
            Duration = duration,
            File = file,
            TrackGenres = new List<TrackGenre>()
        };

        foreach (var genreName in genreNames.Distinct())
        {
            var genre = context.Genres.FirstOrDefault(g => g.Name == genreName) ?? new Genre { Name = genreName };

            if (genre.Id == 0)
            {
                context.Genres.Add(genre);
                context.SaveChanges();
            }

            track.TrackGenres.Add(new TrackGenre { GenreId = genre.Id });
        }

        context.Tracks.Add(track);
        context.SaveChanges();

        return track;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(ReplaceDatabaseImplementation);
        
        return base.CreateHost(builder);
    }

    private static void ReplaceDatabaseImplementation(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<UppbeatDbContext>));

        if (descriptor != null)
            services.Remove(descriptor);

        services.AddDbContext<UppbeatDbContext>(options =>
        {
            options.UseInMemoryDatabase("InMemoryTestDb");
        });
    }

    internal void ClearData()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UppbeatDbContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Uppbeat.Api.Data;

namespace Uppbeat.Api.IntegrationTests;

public class AnonymousWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private IServiceScope _scope;
    private UppbeatDbContext _dbContext;

    public UppbeatDbContext GetDbContext()
    {
        if (_dbContext == null)
        {
            _scope = Services.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<UppbeatDbContext>();
        }

        return _dbContext;
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

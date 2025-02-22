using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Uppbeat.Api.Data;

namespace Uppbeat.Api.IntegrationTests;

public class AnonymousWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
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

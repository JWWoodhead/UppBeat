using Microsoft.EntityFrameworkCore;
using Uppbeat.Api.Data;

namespace Uppbeat.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var startup = new Startup(builder.Configuration);

        startup.ConfigureServices(builder.Services);

        var app = builder.Build();

        startup.Configure(app);

        // Migration scripts are not recommended for Production use but this will suffice for testing purposes
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<UppbeatDbContext>();

            // Check whether current context is a real SQL server instance (not the in-memory provider used in integration tests)
            if (dbContext.Database.IsSqlServer())
                dbContext.Database.Migrate();
        }

        app.Run();
    }
}
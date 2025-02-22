using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.RateLimiting;
using Uppbeat.Api.Data;

namespace Uppbeat.Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<UppbeatDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));
        services.AddControllers();

        services.AddIdentity<UppbeatUser, IdentityRole>()
                .AddEntityFrameworkStores<UppbeatDbContext>()
                .AddDefaultTokenProviders();

        services.AddSwaggerGen(options =>
        {
            var documentationXmlPath = Path.Combine(AppContext.BaseDirectory, "Uppbeat.Api.xml");

            if (File.Exists(documentationXmlPath))
                options.IncludeXmlComments(documentationXmlPath);
        });
    }

    public void Configure(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();
    }
}

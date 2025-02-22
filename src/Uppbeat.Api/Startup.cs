using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using Uppbeat.Api.Common;
using Uppbeat.Api.Data;
using Uppbeat.Api.Repositories;
using Uppbeat.Api.Services;

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

        services.AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1);
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

        services.AddIdentity<UppbeatUser, IdentityRole>()
                .AddEntityFrameworkStores<UppbeatDbContext>()
                .AddDefaultTokenProviders();

        services.AddAuthorization(options =>
                {
                    options.AddPolicy(CustomPolicies.IsArtist, policy => policy.RequireClaim(CustomClaims.ArtistId));
                })
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = _configuration["Auth:ValidAudience"],
                        ValidIssuer = _configuration["Auth:ValidIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Auth:Secret"]))
                    };
                });

        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                // If the user is authenticated, use their identity name;
                // otherwise, fall back to IP or "Unknown".
                var userName = httpContext.User.Identity?.IsAuthenticated == true
                    ? httpContext.User.Identity.Name
                    : null;

                var partitionKey = userName
                    ?? httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "Unknown";

                const int defaultPermitLimit = 100;

                // Should use "Options" config but no time...
                if (!int.TryParse(_configuration["RateLimit:PermitLimit"], out var permitLimit))
                    permitLimit = defaultPermitLimit;

                const int defaultWindowMinutes = 1;

                if (!int.TryParse(_configuration["RateLimit:WindowMinutes"], out var window))
                    window = defaultWindowMinutes;

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = permitLimit,
                        Window = TimeSpan.FromMinutes(window),
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    }
                );
            });
        });

        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IArtistService, ArtistService>();
        services.AddTransient<ITrackService, TrackService>();

        services.AddTransient<IArtistRepository, ArtistRepository>();
        services.AddTransient<IGenreRepository, GenreRepository>();
        services.AddTransient<ITrackRepository, TrackRepository>();

        services.AddEndpointsApiExplorer();
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

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}

using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Uppbeat.Api.Data;
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

        services.AddAuthentication(options =>
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

        services.AddTransient<IUserService, UserService>();

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

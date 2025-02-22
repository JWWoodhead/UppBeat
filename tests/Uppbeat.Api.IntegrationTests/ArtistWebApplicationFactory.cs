using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;
using Uppbeat.Api.Common;

namespace Uppbeat.Api.IntegrationTests;

public class ArtistWebApplicationFactory<TStartup> : AnonymousWebApplicationFactory<TStartup> where TStartup : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(ReplaceAuthImplementation);

        return base.CreateHost(builder);
    }

    private static void ReplaceAuthImplementation(IServiceCollection services)
    {
        var authServiceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAuthenticationService));

        if (authServiceDescriptor != null)
            services.Remove(authServiceDescriptor);

        services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.TestScheme;
                    options.DefaultChallengeScheme = TestAuthHandler.TestScheme;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.TestScheme, options => { });

        // This is duplicated across Startup and the ArtistWebApplicationFactory. Should be refactored but already low on time...
        services.AddAuthorization(options =>
        {
            options.AddPolicy(CustomPolicies.IsArtist, policy => policy.RequireClaim(CustomClaims.ArtistId));
        });
    }

    protected HttpClient CreateClient(WebApplicationFactoryClientOptions options)
    {
        var client = base.CreateClient(options);

        // Add authorization header to simulate authenticated artist user
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.TestScheme);

        return client;
    }
}
using System.Net.Http.Json;
using System.Net;
using Uppbeat.Api.Models.Auth;

namespace Uppbeat.Api.IntegrationTests.AuthController;

[Collection(nameof(SequentialTestsCollection))]
public class AuthController_LoginTests : IClassFixture<AnonymousWebApplicationFactory<Startup>>
{
    private AnonymousWebApplicationFactory<Startup> _testFactory;

    public AuthController_LoginTests(
        AnonymousWebApplicationFactory<Startup> testFactory)
    {
        _testFactory = testFactory;
        _testFactory.ClearData();
    }


    [Fact]
    public async Task Login_ValidCredentials_Returns200WithToken()
    {
        var client = _testFactory.CreateClient();
        var registerModel = new RegisterUserRequest
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "ValidPass123!",
        };

        await client.PostAsJsonAsync("/api/v1/auth/register", registerModel);

        var loginModel = new LoginUserRequest
        {
            Username = "testuser",
            Password = "ValidPass123!"
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", loginModel);
        var result = await response.Content.ReadFromJsonAsync<LoginUserResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task Login_InvalidUsername_Returns401()
    {
        var client = _testFactory.CreateClient();
        var loginModel = new LoginUserRequest
        {
            Username = "nonexistent",
            Password = "ValidPass123!"
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", loginModel);
        var error = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Contains("Invalid username", error);
    }

    [Fact]
    public async Task Login_InvalidPassword_Returns401()
    {
        var client = _testFactory.CreateClient();
        var registerModel = new RegisterUserRequest
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "ValidPass123!"
        };

        await client.PostAsJsonAsync("/api/v1/auth/register", registerModel);

        var loginModel = new LoginUserRequest
        {
            Username = "testuser",
            Password = "WrongPass123!"
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", loginModel);
        var error = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Contains("Invalid password", error);
    }
}

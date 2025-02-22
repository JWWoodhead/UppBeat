using System.Net.Http.Json;
using System.Net;
using Uppbeat.Api.Models.Auth;

namespace Uppbeat.Api.IntegrationTests.AuthController;

[Collection(nameof(SequentialTestsCollection))]
public class AuthController_RegisterTests : IClassFixture<AnonymousWebApplicationFactory<Startup>>
{
    private AnonymousWebApplicationFactory<Startup> _testFactory;

    public AuthController_RegisterTests(
        AnonymousWebApplicationFactory<Startup> testFactory)
    {
        _testFactory = testFactory;
        _testFactory.ClearData();
    }

    [Fact]
    public async Task Register_ValidData_Returns200()
    {
        var client = _testFactory.CreateClient();
        var requestModel = new RegisterUserRequest
        {
            Username = "NewUser",
            Email = "newuser@example.com",
            Password = "ValidPass123!",
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", requestModel);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_DuplicateUsername_Returns400()
    {
        var client = _testFactory.CreateClient();
        var requestModel = new RegisterUserRequest
        {
            Username = "ExistingUser",
            Email = "existinguser@example.com",
            Password = "ValidPass123!"
        };

        await client.PostAsJsonAsync("/api/v1/auth/register", requestModel);

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", requestModel);
        var error = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("User already exists", error);
    }

    [Fact]
    public async Task Register_InvalidPassword_Returns500()
    {
        var client = _testFactory.CreateClient();
        var requestModel = new RegisterUserRequest
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Password = "weak"  // Does not meet ASP.NET Core Identity password requirements
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", requestModel);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}

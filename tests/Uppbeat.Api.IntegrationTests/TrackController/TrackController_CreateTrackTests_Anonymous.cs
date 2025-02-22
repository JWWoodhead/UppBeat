using System.Net;
using System.Net.Http.Json;
using Uppbeat.Api.Data;
using Uppbeat.Api.Models.Track;

namespace Uppbeat.Api.IntegrationTests.TrackController;

[Collection(nameof(SequentialTestsCollection))]
public class TrackController_CreateTrackTests_Anonymous : IClassFixture<AnonymousWebApplicationFactory<Startup>>
{
    private AnonymousWebApplicationFactory<Startup> _testFactory;

    public TrackController_CreateTrackTests_Anonymous(
        AnonymousWebApplicationFactory<Startup> testFactory)
    {
        _testFactory = testFactory;
        _testFactory.ClearData();
    }

    [Fact]
    public async Task CreateTrack_UnauthorizedUser_Returns401()
    {
        var artist = _testFactory.CreateArtist(new Artist { Name = "Anonymous Artist" });
        var client = _testFactory.CreateClient();

        var requestModel = new CreateTrackRequest
        {
            Name = "Should Fail",
            Genres = new List<string> { "Test" },
            Duration = 200,
            File = "test.mp3",
            Artist = artist.Id
        };

        var response = await client.PostAsJsonAsync("/api/v1/tracks", requestModel);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

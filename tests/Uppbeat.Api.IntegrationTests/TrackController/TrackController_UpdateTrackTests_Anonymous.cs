using System.Net;
using System.Net.Http.Json;
using Uppbeat.Api.Data;
using Uppbeat.Api.Models.Track;

namespace Uppbeat.Api.IntegrationTests.TrackController;

[Collection(nameof(SequentialTestsCollection))]
public class TrackController_UpdateTrackTests_Anonymous : IClassFixture<AnonymousWebApplicationFactory<Startup>>
{
    private AnonymousWebApplicationFactory<Startup> _testFactory;

    public TrackController_UpdateTrackTests_Anonymous(AnonymousWebApplicationFactory<Startup> testFactory)
    {
        _testFactory = testFactory;
        _testFactory.ClearData();
    }

    [Fact]
    public async Task UpdateTrack_UnauthorizedUser_Returns401()
    {
        var artist = _testFactory.CreateArtist(new Artist { Name = "Anonymous Artist" });
        var client = _testFactory.CreateClient();

        var requestModel = new UpdateTrackRequest
        {
            Name = "Should Fail",
            Genres = new List<string> { "Test" },
            Duration = 200,
            File = "test.mp3"
        };

        var response = await client.PutAsJsonAsync($"/api/v1/tracks/1", requestModel);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

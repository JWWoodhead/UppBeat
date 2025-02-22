using System.Net;
using Uppbeat.Api.Data;

namespace Uppbeat.Api.IntegrationTests.TrackController;

[Collection(nameof(SequentialTestsCollection))]
public class TrackController_DeleteTrackTests_Anonymous : IClassFixture<AnonymousWebApplicationFactory<Startup>>
{
    private AnonymousWebApplicationFactory<Startup> _testFactory;

    public TrackController_DeleteTrackTests_Anonymous(
        AnonymousWebApplicationFactory<Startup> testFactory)
    {
        _testFactory = testFactory;
        _testFactory.ClearData();
    }

    [Fact]
    public async Task DeleteTrack_UnauthorizedUser_Returns401()
    {
        var artist = _testFactory.CreateArtist(new Artist { Name = "Test Artist" });
        var track = _testFactory.CreateTrack(new Track { Name = "Track 1", ArtistId = artist.Id, File = "Track1.wav" });
        var client = _testFactory.CreateClient();

        var response = await client.DeleteAsync($"/api/v1/tracks/{track.Id}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

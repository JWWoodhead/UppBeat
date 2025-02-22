using System.Net;
using Uppbeat.Api.Data;

namespace Uppbeat.Api.IntegrationTests.TrackController;

[Collection(nameof(SequentialTestsCollection))]
public class TrackController_DeleteTrackTests_Artist : IClassFixture<ArtistWebApplicationFactory<Startup>>
{
    private ArtistWebApplicationFactory<Startup> _testFactory;

    public TrackController_DeleteTrackTests_Artist(
        ArtistWebApplicationFactory<Startup> testFactory)
    {
        _testFactory = testFactory;
        _testFactory.ClearData();
    }

    [Fact]
    public async Task DeleteTrack_ForExistingTrack_Returns204()
    {
        var artist = _testFactory.CreateArtist(new Artist { Id = TestAuthHandler.DefaultClaimPolicyArtistId, Name = "Test Artist" });
        var track = _testFactory.CreateTrack(new Track { Name = "Track 1", ArtistId = artist.Id, File = "Track1.wav" });
        var client = _testFactory.CreateClient();

        var response = await client.DeleteAsync($"/api/v1/tracks/{track.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTrack_ForExistingTrack_WithDifferentArtistId_Returns400()
    {
        var artist = _testFactory.CreateArtist(new Artist { Id = 234, Name = "Test Artist" });
        var track = _testFactory.CreateTrack(new Track { Name = "Track 1", ArtistId = artist.Id, File = "Track1.wav" });
        var client = _testFactory.CreateClient();

        var response = await client.DeleteAsync($"/api/v1/tracks/{track.Id}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

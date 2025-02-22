using System.Net;
using System.Net.Http.Json;
using Uppbeat.Api.Data;
using Uppbeat.Api.Models.Track;

namespace Uppbeat.Api.IntegrationTests.TrackController;

[Collection(nameof(SequentialTestsCollection))]
public class TrackController_GetTrackTests_Anonymous : IClassFixture<ArtistWebApplicationFactory<Startup>>
{
    private readonly ArtistWebApplicationFactory<Startup> _testFactory;

    public TrackController_GetTrackTests_Anonymous(ArtistWebApplicationFactory<Startup> testFactory)
    {
        _testFactory = testFactory;
        _testFactory.ClearData();
    }

    [Fact]
    public async Task GetTrack_ExistingId_ReturnsExpectedTrack()
    {
        var artist = _testFactory.CreateArtist(new Artist { Name = "Bal Sagoth" });
        var expectedTrack = _testFactory.CreateTrackWithGenres(
            "Thwarted by the Dark (Blade of the Vampyre Hunter)",
            artist,
            500,
            "balsagoth_thwarted.mp3",
            ["Metal"]);

        var client = _testFactory.CreateClient();
        var response = await client.GetAsync($"/api/v1/tracks/{expectedTrack.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var track = await response.Content.ReadFromJsonAsync<GetTrackResponse>();

        Assert.NotNull(track);
        Assert.Equal(expectedTrack.Id, track.Id);
        Assert.Equal("Thwarted by the Dark (Blade of the Vampyre Hunter)", track.Name);
        Assert.Contains("Metal", track.Genres);
        Assert.True(track.Duration > 0);
    }

    [Fact]
    public async Task GetTrack_NonExistentId_ReturnsNotFound()
    {
        var client = _testFactory.CreateClient();
        var nonExistentId = 999999;
        var response = await client.GetAsync($"/api/v1/tracks/{nonExistentId}");
        var message = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains($"Track with ID {nonExistentId} not found", message);
    }
}

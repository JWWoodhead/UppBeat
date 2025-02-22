using System.Net;
using System.Net.Http.Json;
using Uppbeat.Api.Data;
using Uppbeat.Api.Models.Track;

namespace Uppbeat.Api.IntegrationTests.TrackController;

[Collection(nameof(SequentialTestsCollection))]
public class TrackController_CreateTrackTests_Artist : IClassFixture<ArtistWebApplicationFactory<Startup>>
{
    private ArtistWebApplicationFactory<Startup> _testFactory;

    public TrackController_CreateTrackTests_Artist(
        ArtistWebApplicationFactory<Startup> testFactory)
    {
        _testFactory = testFactory;
        _testFactory.ClearData();
    }

    [Fact]
    public async Task CreateTrack_ValidData_Returns201()
    {
        var artist = _testFactory.CreateArtist(new Artist { Name = "ArtistUser" });
        var rockGenre = _testFactory.CreateGenre(new Genre { Name = "Rock" });
        var indieGenre = _testFactory.CreateGenre(new Genre { Name = "Indie" });
        var client = _testFactory.CreateClient();

        var requestModel = new CreateTrackRequest
        {
            Name = "My New Song",
            Genres = new List<string> { rockGenre.Name, indieGenre.Name },
            Duration = 300,
            File = "mysong.mp3",
            Artist = artist.Id
        };

        var response = await client.PostAsJsonAsync("/api/v1/tracks", requestModel);
        var result = await response.Content.ReadFromJsonAsync<CreateTrackResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("My New Song", result.Name);
    }

    [Fact]
    public async Task CreateTrack_InvalidData_Returns400()
    {
        var client = _testFactory.CreateClient();

        var invalidRequest = new CreateTrackRequest
        {
            Name = "",
            Genres = new List<string>(),
            Duration = 0
        };

        var response = await client.PostAsJsonAsync("/api/v1/tracks", invalidRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTrack_UnauthorizedUser_Returns400_WithNonExistantArtist()
    {
        var client = _testFactory.CreateClient();
        var requestModel = new CreateTrackRequest
        {
            Name = "Should Fail",
            Artist = 345,
            Genres = new List<string> { "Test" },
            Duration = 200,
            File = "test.mp3"
        };

        var response = await client.PostAsJsonAsync("/api/v1/tracks", requestModel);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

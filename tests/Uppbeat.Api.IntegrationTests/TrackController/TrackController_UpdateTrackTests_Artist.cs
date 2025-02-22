using System.Net;
using System.Net.Http.Json;
using Uppbeat.Api.Data;
using Uppbeat.Api.Models.Track;

namespace Uppbeat.Api.IntegrationTests.TrackController;

[Collection(nameof(SequentialTestsCollection))]
public class TrackController_UpdateTrackTests_Artist : IClassFixture<ArtistWebApplicationFactory<Startup>>
{
    private ArtistWebApplicationFactory<Startup> _testFactory;

    public TrackController_UpdateTrackTests_Artist(ArtistWebApplicationFactory<Startup> testFactory)
    {
        _testFactory = testFactory;
        _testFactory.ClearData();
    }

    [Fact]
    public async Task UpdateTrack_ExistingTrack_Returns204NoContent()
    {
        var artist = _testFactory.CreateArtist(new Artist { Id = TestAuthHandler.DefaultClaimPolicyArtistId, Name = "Artist" });

        var track = _testFactory.CreateTrackWithGenres(
            "Old Name",
            artist,
            300,
            "oldfile.mp3",
            new[] { "Rock" }
        );

        var client = _testFactory.CreateClient();

        var requestModel = new UpdateTrackRequest
        {
            Name = "Updated Track Name",
            Genres = new List<string> { "Rock" },
            Duration = 999,
            File = "updatedFile.mp3"
        };

        var response = await client.PutAsJsonAsync($"/api/v1/tracks/{track.Id}", requestModel);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    // Cannot test without being able to update ArtistId Claim in custom auth provider. Commenting out for now due to time constraints
    //[Fact]
    //public async Task UpdateTrack_ForDifferentArtist_Returns400BadRequest()
    //{
    //    var artist1 = _testFactory.CreateArtist(new Artist { Name = "Artist1" });
    //    var artist2 = _testFactory.CreateArtist(new Artist { Name = "Artist2" });

    //    var track = _testFactory.CreateTrackWithGenres(
    //        "Old Name",
    //        artist1,
    //        300,
    //        "file.mp3",
    //        new[] { "Rock" }
    //    );

    //    var client = _testFactory.CreateClient();
    //    var requestModel = new UpdateTrackRequest
    //    {
    //        Name = "Unauthorized Update",
    //        Genres = new List<string> { "Rock" },
    //        Duration = 400,
    //        File = "dummy.mp3"
    //    };

    //    var response = await client.PutAsJsonAsync($"/api/tracks/{track.Id}", requestModel);
    //    var message = await response.Content.ReadAsStringAsync();

    //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    //    Assert.Contains("You are not authorized to update this track", message);
    //}

    [Fact]
    public async Task UpdateTrack_ForNoneExistantGenre_Returns400BadRequest()
    {
        // Create artist with same Id as hard coded claim in ArtistWebApplicationFactory
        var artist = _testFactory.CreateArtist(new Artist { Id = TestAuthHandler.DefaultClaimPolicyArtistId, Name = "Artist" });

        var track = _testFactory.CreateTrackWithGenres(
            "Old Name",
            artist,
            300,
            "file.mp3",
            new[] { "Metal" }
        );

        var client = _testFactory.CreateClient();
        var requestModel = new UpdateTrackRequest
        {
            Name = "Unauthorized Update",
            Genres = new List<string> { "Test" },
            Duration = 400,
            File = "dummy.mp3"
        };

        var response = await client.PutAsJsonAsync($"/api/v1/tracks/{track.Id}", requestModel);
        var message = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains($"The following genres were not found: Test", message);
    }

    [Fact]
    public async Task UpdateTrack_NonExistentTrack_Returns404NotFound()
    {
        var client = _testFactory.CreateClient();
        var nonExistentId = 999999;

        var requestModel = new UpdateTrackRequest
        {
            Name = "DoesNotExist",
            Genres = new List<string> { "Test" },
            Duration = 300,
            File = "none.mp3"
        };

        var response = await client.PutAsJsonAsync($"/api/v1/tracks/{nonExistentId}", requestModel);
        var message = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains($"Track with ID {nonExistentId} not found", message);
    }
}

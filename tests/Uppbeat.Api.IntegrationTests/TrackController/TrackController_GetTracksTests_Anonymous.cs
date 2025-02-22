using System.Net;
using System.Net.Http.Json;
using Uppbeat.Api.Data;
using Uppbeat.Api.Models.Track;

namespace Uppbeat.Api.IntegrationTests.TrackController;

[Collection(nameof(SequentialTestsCollection))]
public class TrackController_GetTracksTests_Anonymous : IClassFixture<AnonymousWebApplicationFactory<Startup>>
{
    private readonly AnonymousWebApplicationFactory<Startup> _testFactory;

    public TrackController_GetTracksTests_Anonymous(AnonymousWebApplicationFactory<Startup> testFactory)
    {
        _testFactory = testFactory;
        _testFactory.ClearData();

        var balSagoth = _testFactory.CreateArtist(new Artist { Name = "Bal Sagoth" });
        var milesDavis = _testFactory.CreateArtist(new Artist { Name = "Miles Davis" });
        var bobMarley = _testFactory.CreateArtist(new Artist { Name = "Bob Marley" });
        var daftPunk = _testFactory.CreateArtist(new Artist { Name = "Daft Punk" });
        var theBeatles = _testFactory.CreateArtist(new Artist { Name = "The Beatles" });
        var ledZeppelin = _testFactory.CreateArtist(new Artist { Name = "Led Zeppelin" });
        var eminem = _testFactory.CreateArtist(new Artist { Name = "Eminem" });
        var greenDay = _testFactory.CreateArtist(new Artist { Name = "Green Day" });

        _testFactory.CreateTrackWithGenres("The Dark Liege of Chaos Is Unleashed...", balSagoth, 180, "balsagoth_darkliege.mp3", new[] { "Metal" });
        _testFactory.CreateTrackWithGenres("Thwarted by the Dark (Blade of the Vampyre Hunter)", balSagoth, 500, "balsagoth_thwarted.mp3", new[] { "Metal" });
        _testFactory.CreateTrackWithGenres("Cry Havoc for Glory, ... (Part III)", balSagoth, 430, "balsagoth_havoc.mp3", new[] { "Metal" });
        _testFactory.CreateTrackWithGenres("Blue in Green", milesDavis, 330, "miles_blue.mp3", new[] { "Jazz", "Fusion" });
        _testFactory.CreateTrackWithGenres("So What", milesDavis, 560, "miles_so.mp3", new[] { "Jazz" });
        _testFactory.CreateTrackWithGenres("One Love", bobMarley, 180, "marley_one.mp3", new[] { "Reggae" });
        _testFactory.CreateTrackWithGenres("Three Little Birds", bobMarley, 210, "marley_three.mp3", new[] { "Reggae" });
        _testFactory.CreateTrackWithGenres("Around the World", daftPunk, 315, "daftpunk_around.mp3", new[] { "Electronic", "Funk" });
        _testFactory.CreateTrackWithGenres("Harder, Better, Faster, Stronger", daftPunk, 220, "daftpunk_harder.mp3", new[] { "Electronic", "Funk" });
        _testFactory.CreateTrackWithGenres("Hey Jude", theBeatles, 431, "beatles_heyjude.mp3", new[] { "Rock" });
        _testFactory.CreateTrackWithGenres("Come Together", theBeatles, 259, "beatles_come.mp3", new[] { "Rock" });
        _testFactory.CreateTrackWithGenres("Stairway to Heaven", ledZeppelin, 482, "zep_stairway.mp3", new[] { "Rock" });
        _testFactory.CreateTrackWithGenres("Kashmir", ledZeppelin, 510, "zep_kashmir.mp3", new[] { "Rock" });
        _testFactory.CreateTrackWithGenres("Lose Yourself", eminem, 326, "eminem_lose.mp3", new[] { "Hip Hop" });
        _testFactory.CreateTrackWithGenres("Not Afraid", eminem, 259, "eminem_notafraid.mp3", new[] { "Hip Hop" });
        _testFactory.CreateTrackWithGenres("Basketcase", greenDay, 200, "greenday_basketcase.mp3", new[] { "Pop", "Punk" });
    }

    [Fact]
    public async Task GetTracks_NoFilters_ReturnsAllTracksInDefaultPage()
    {
        var client = _testFactory.CreateClient();
        var response = await client.GetAsync("/api/v1/tracks");
        var result = await response.Content.ReadFromJsonAsync<GetTracksResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(16, result.TotalCount);
        Assert.NotNull(result.Tracks);
        Assert.Equal(10, result.Tracks.Count());
    }

    [Fact]
    public async Task GetTracks_FilterByGenre_ReturnsOnlyMatchingTracks()
    {
        var client = _testFactory.CreateClient();
        var genre = "Metal";
        var response = await client.GetAsync($"/api/v1/tracks?genre={genre}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GetTracksResponse>();
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.All(result.Tracks, t => Assert.Contains(genre, t.Genres));
    }

    [Fact]
    public async Task GetTracks_FilterBySearch_ReturnsOnlyMatchingTracks()
    {
        var client = _testFactory.CreateClient();
        var searchTerm = "Green";
        var response = await client.GetAsync($"/api/v1/tracks?search={searchTerm}");
        var result = await response.Content.ReadFromJsonAsync<GetTracksResponse>();

        response.EnsureSuccessStatusCode();
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount); // One matching artist and one matching track
        Assert.Equal(2, result.Tracks.Count());
    }

    [Fact]
    public async Task GetTracks_CustomPagination_ReturnsCorrectPage()
    {
        var client = _testFactory.CreateClient();
        var page = 2;
        var pageSize = 1;
        var response = await client.GetAsync($"/api/v1/tracks?page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GetTracksResponse>();
        Assert.NotNull(result);
        Assert.Equal(page, result.Page);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Single(result.Tracks);
    }

    [Fact]
    public async Task GetTracks_NonExistentGenre_ReturnsEmptyList()
    {
        var client = _testFactory.CreateClient();
        var nonExistentGenre = "NotAGenre";
        var response = await client.GetAsync($"/api/v1/tracks?genre={nonExistentGenre}");
        var result = await response.Content.ReadFromJsonAsync<GetTracksResponse>();

        response.EnsureSuccessStatusCode();
        Assert.NotNull(result);
        Assert.Empty(result.Tracks);
        Assert.Equal(0, result.TotalCount);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(0, 1)]
    [InlineData(101, 0)]
    [InlineData(0, 101)]
    public async Task GetTracks_InvalidPagination_ReturnsBadRequest(int pageNumber, int pageSize)
    {
        var client = _testFactory.CreateClient();
        var response = await client.GetAsync("/api/v1/tracks?page=0&pageSize=0");
        var result = await response.Content.ReadFromJsonAsync<GetTracksResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

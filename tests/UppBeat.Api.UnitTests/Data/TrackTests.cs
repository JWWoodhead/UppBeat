using Uppbeat.Api.Data;
using Uppbeat.Api.Models.Track;

namespace Uppbeat.Api.UnitTests.Data;

public class TrackTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_FromCreateTrackRequest()
    {
        var createRequest = new CreateTrackRequest
        {
            Name = "Test Track",
            Artist = 123,
            Duration = 200,
            File = "test.mp3",
            Genres = new List<string> { "Pop", "Rock" }
        };

        var genres = new List<Genre>
        {
            new Genre { Id = 1000, Name = "Pop" },
            new Genre { Id = 1001, Name = "Rock" }
        };

        var track = new Track(createRequest, genres);

        Assert.Equal("Test Track", track.Name);
        Assert.Equal(123, track.ArtistId);
        Assert.Equal(200, track.Duration);
        Assert.Equal("test.mp3", track.File);
        Assert.Equal(2, track.TrackGenres.Count);
        Assert.Contains("Pop", track.TrackGenres.Select(tg => tg.Genre.Name));
        Assert.Contains("Rock", track.TrackGenres.Select(tg => tg.Genre.Name));
    }
}

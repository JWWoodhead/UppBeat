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

    [Fact]
    public void Update_ShouldModifyProperties_FromUpdateTrackRequest()
    {
        var track = new Track
        {
            Id = 1,
            Name = "Original Name",
            ArtistId = 234,
            Duration = 180,
            File = "original.mp3",
            TrackGenres = new List<TrackGenre>
            {
                new TrackGenre { Genre = new Genre { Name = "Hip-Hop"  } },
            }
        };

        var updateRequest = new UpdateTrackRequest
        {
            Name = "Updated Name",
            Duration = 300,
            File = "updated.mp3",
            Genres = new List<string> { "Pop", "Electro" }
        };

        var matchingGenres = new List<Genre>
        {
            new Genre { Id = 1, Name = "Pop" },
            new Genre { Id = 2, Name = "Electro" }
        };

        track.Update(updateRequest, matchingGenres);

        Assert.Equal("Updated Name", track.Name);
        Assert.Equal(300, track.Duration);
        Assert.Equal("updated.mp3", track.File);
        Assert.Equal(2, track.TrackGenres.Count);
        Assert.Contains("Pop", track.TrackGenres.Select(tg => tg.Genre.Name));
        Assert.Contains("Electro", track.TrackGenres.Select(tg => tg.Genre.Name));
    }
}

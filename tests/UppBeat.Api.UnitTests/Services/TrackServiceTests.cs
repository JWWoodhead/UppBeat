using NSubstitute;
using Uppbeat.Api.Data;
using Uppbeat.Api.Models.Artist;
using Uppbeat.Api.Models.Track;
using Uppbeat.Api.Repositories;
using Uppbeat.Api.Services;

namespace Uppbeat.Api.UnitTests.Services;

public class TrackServiceTests
{
    private readonly ITrackRepository _trackRepository;
    private readonly IArtistService _artistService;
    private readonly IGenreRepository _genreRepository;
    private readonly TrackService _trackService;

    public TrackServiceTests()
    {
        _artistService = Substitute.For<IArtistService>();
        _trackRepository = Substitute.For<ITrackRepository>();
        _genreRepository = Substitute.For<IGenreRepository>();
        _trackService = new TrackService(
            _artistService,
            _genreRepository,
            _trackRepository);
    }

    [Fact]
    public async Task CreateTrackAsync_WithMixOfValidAndInvalidGenres_ReturnsBadRequest()
    {
        var artist = new ReadArtistModel { Id = 1 };

        _artistService
            .GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(artist);

        _genreRepository
            .GetGenresByNames(Arg.Any<List<string>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Genre> { new Genre { Name = "Rock" } });

        var request = new CreateTrackRequest
        {
            Name = "New Track",
            Artist = 1,
            Genres = new List<string> { "Rock", "Pop" }
        };

        var result = await _trackService.CreateTrackAsync(request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Pop", result.Error); // Verify exact missing genre is mentioned
    }

    [Fact]
    public async Task UpdateTrackAsync_WhenArtistIdDoesNotMatch_ReturnsBadRequest()
    {
        var existingTrack = new Track
        {
            Id = 1,
            ArtistId = 1,
            Name = "Original Track"
        };

        _trackRepository
            .GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(existingTrack);

        var request = new UpdateTrackRequest
        {
            Name = "Updated Track"
        };
        var newArtistId = 2;

        var result = await _trackService.UpdateTrackAsync(1, newArtistId, request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not authorized", result.Error);

        // Verify no update was attempted
        await _trackRepository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<Track>(), Arg.Any<CancellationToken>());
    }
}

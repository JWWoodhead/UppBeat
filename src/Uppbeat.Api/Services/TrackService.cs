using Uppbeat.Api.Common;
using Uppbeat.Api.Data;
using Uppbeat.Api.Models.Track;
using Uppbeat.Api.Repositories;

namespace Uppbeat.Api.Services;

public class TrackService : ITrackService
{
    private readonly IArtistService _artistService;
    private readonly IGenreRepository _genreRepository;
    private readonly ITrackRepository _trackRepository;

    public TrackService(
        IArtistService artistService,
        IGenreRepository genreRepository,
        ITrackRepository trackRepository)
    {
        _artistService = artistService;
        _genreRepository = genreRepository;
        _trackRepository = trackRepository;
    }

    public async Task<Result<CreateTrackResponse>> CreateTrackAsync(CreateTrackRequest createTrackRequest, CancellationToken cancellationToken)
    {
        var artist = await _artistService.GetByIdAsync(createTrackRequest.Artist, cancellationToken);

        if (artist == null)
            return Result<CreateTrackResponse>.BadRequest($"Specified Artist with ID {createTrackRequest.Artist} does not exist. Please specify a valid Artist");

        var genreNames = createTrackRequest.Genres
            .Distinct()
            .ToList();

        var matchingGenres = await _genreRepository.GetGenresByNames(genreNames, cancellationToken);

        // Identify genres specified that do not exist.
        var missingGenres = genreNames
            .Except(matchingGenres.Select(g => g.Name))
            .ToList();

        if (missingGenres.Any())
            return Result<CreateTrackResponse>.BadRequest($"The following genres were not found: {string.Join(", ", missingGenres)}");

        var track = new Track(createTrackRequest, matchingGenres);

        await _trackRepository.CreateAsync(track, cancellationToken);

        return Result<CreateTrackResponse>.Success(new CreateTrackResponse
        {
            Id = track.Id,
            Artist = track.ArtistId,
            Duration = track.Duration,
            File = track.File,
            Name = track.Name,
            Genres = track.TrackGenres
                .Select(tg => tg.Genre.Name)
                .ToList(),
        });
    }

    public async Task<Result<GetTrackResponse>> GetTrackByIdAsync(int id, CancellationToken cancellationToken)
    {
        var track = await _trackRepository.GetByIdAsync(id, cancellationToken);

        if (track == null)
            return Result<GetTrackResponse>.NotFound($"Track with ID {id} not found");

        return Result<GetTrackResponse>.Success(new GetTrackResponse
        {
            Id = id,
            Name = track.Name,
            ArtistId = track.ArtistId,
            Duration = track.Duration,
            File = track.File,
            Genres = track.TrackGenres
                .Select(tg => tg.Genre.Name)
                .ToList()
        });
    }

    public async Task<Result> UpdateTrackAsync(int id, int artistId, UpdateTrackRequest updateTrackModel, CancellationToken cancellationToken)
    {
        var track = await _trackRepository.GetByIdAsync(id, cancellationToken);

        if (track == null)
            return Result.NotFound($"Track with ID {id} not found");

        if (track.ArtistId != artistId)
            return Result.BadRequest("You are not authorized to update this track");

        var genreNames = updateTrackModel.Genres
            .Distinct()
            .ToList();

        var matchingGenres = await _genreRepository.GetGenresByNames(genreNames, cancellationToken);

        // Identify genres specified that do not exist.
        var missingGenres = genreNames
            .Except(matchingGenres.Select(g => g.Name))
            .ToList();

        if (missingGenres.Any())
            return Result.BadRequest($"The following genres were not found: {string.Join(", ", missingGenres)}");

        track.Update(updateTrackModel, matchingGenres);

        await _trackRepository.UpdateAsync(track, cancellationToken);

        return Result.Success();
    }

}

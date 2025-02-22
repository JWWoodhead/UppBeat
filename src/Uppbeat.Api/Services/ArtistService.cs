using Uppbeat.Api.Data;
using Uppbeat.Api.Models.Artist;
using Uppbeat.Api.Repositories;

namespace Uppbeat.Api.Services;

public class ArtistService : IArtistService
{
    private readonly IArtistRepository _artistRepository;

    public ArtistService(
        IArtistRepository artistRepository)
    {
        _artistRepository = artistRepository;
    }

    public async Task<ReadArtistModel?> GetByIdAsync(int artistId, CancellationToken cancellationToken)
    {
        var artist = await _artistRepository.GetByIdAsync(artistId, cancellationToken);

        return artist == null 
            ? null
            : MapToReadModel(artist);
    }

    public async Task<ReadArtistModel?> GetByNameAsync(string artistName, CancellationToken cancellationToken)
    {
        var artist = await _artistRepository.GetByNameAsync(artistName, cancellationToken);

        return artist == null
            ? null
            : MapToReadModel(artist);
    }

    public async Task<ReadArtistModel> CreateAsync(string artistName, CancellationToken cancellationToken)
    {
        var artist = new Artist(artistName);

        await _artistRepository.CreateAsync(artist, cancellationToken);

        return MapToReadModel(artist);
    }

    private static ReadArtistModel MapToReadModel(Artist artist)
    {
        return new ReadArtistModel
        {
            Id = artist.Id,
            Name = artist.Name,
        };
    }
}

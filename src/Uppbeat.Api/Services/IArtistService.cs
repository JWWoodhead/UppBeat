using Uppbeat.Api.Models.Artist;

namespace Uppbeat.Api.Services;

public interface IArtistService
{
    Task<ReadArtistModel?> GetByIdAsync(int artist, CancellationToken cancellationToken);

    Task<ReadArtistModel?> GetByNameAsync(string artistName, CancellationToken cancellationToken);

    Task<ReadArtistModel> CreateAsync(string artistName, CancellationToken cancellationToken);
}

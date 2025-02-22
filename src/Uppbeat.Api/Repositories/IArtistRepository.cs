using Uppbeat.Api.Data;

namespace Uppbeat.Api.Repositories;

public interface IArtistRepository
{
    Task<Artist?> GetByIdAsync(int artistId, CancellationToken cancellationToken);

    Task<Artist?> GetByNameAsync(string artistName, CancellationToken cancellationToken);

    Task<Artist> CreateAsync(Artist artist, CancellationToken cancellationToken);
}

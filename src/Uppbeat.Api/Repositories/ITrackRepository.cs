using Uppbeat.Api.Data;

namespace Uppbeat.Api.Repositories;

public interface ITrackRepository
{
    Task<Track?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<Track> CreateAsync(Track track, CancellationToken cancellationToken);

    Task UpdateAsync(Track track, CancellationToken cancellationToken);
}
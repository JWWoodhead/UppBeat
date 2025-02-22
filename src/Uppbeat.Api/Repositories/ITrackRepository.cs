using Uppbeat.Api.Data;

namespace Uppbeat.Api.Repositories;

public interface ITrackRepository
{
    Task<Track?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<(IEnumerable<Track> tracks, int totalCount)> GetTracksAsync(string? genre, string? search, int page, int pageSize, CancellationToken cancellationToken);

    Task<Track> CreateAsync(Track track, CancellationToken cancellationToken);

    Task UpdateAsync(Track track, CancellationToken cancellationToken);

    Task DeleteAsync(Track track, CancellationToken cancellationToken);
}
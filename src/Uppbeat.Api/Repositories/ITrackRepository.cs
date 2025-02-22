using Uppbeat.Api.Data;

namespace Uppbeat.Api.Repositories;

public interface ITrackRepository
{
    Task<Track> CreateAsync(Track track, CancellationToken cancellationToken);
}
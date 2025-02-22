using Uppbeat.Api.Data;

namespace Uppbeat.Api.Repositories;

public class TrackRepository : ITrackRepository
{
    private readonly UppbeatDbContext _context;

    public TrackRepository(UppbeatDbContext context)
    {
        _context = context;
    }

    public async Task<Track> CreateAsync(Track track, CancellationToken cancellationToken)
    {
        _context.Tracks.Add(track);

        await _context.SaveChangesAsync(cancellationToken);

        return track;
    }
}

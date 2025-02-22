using Microsoft.EntityFrameworkCore;
using Uppbeat.Api.Data;

namespace Uppbeat.Api.Repositories;

public class TrackRepository : ITrackRepository
{
    private readonly UppbeatDbContext _context;

    public TrackRepository(UppbeatDbContext context)
    {
        _context = context;
    }

    public async Task<Track?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Tracks
            .Include(t => t.Artist)
            .Include(t => t.TrackGenres)
            .ThenInclude(tg => tg.Genre)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Track> CreateAsync(Track track, CancellationToken cancellationToken)
    {
        _context.Tracks.Add(track);

        await _context.SaveChangesAsync(cancellationToken);

        return track;
    }

    public async Task UpdateAsync(Track track, CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}

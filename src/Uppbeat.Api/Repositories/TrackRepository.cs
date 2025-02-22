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

    public async Task<(IEnumerable<Track> tracks, int totalCount)> GetTracksAsync(string? genre, string? search, int page, int pageSize, CancellationToken cancellationToken)
    {
        IQueryable<Track> query = _context.Tracks
            .Include(t => t.Artist)
            .Include(t => t.TrackGenres)
            .ThenInclude(tg => tg.Genre);

        if (!string.IsNullOrEmpty(genre))
        {
            query = query.Where(t => t.TrackGenres.Any(tg => tg.Genre.Name == genre));
        }

        // Poor implementation of search - should really use a full text index but this isn't supported by the in-memory provider.
        // The SQL server full text is also pretty bad and hasn't been updated in years so I'd probably recommend something like ElasticSearch
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(t =>
                t.Name.Contains(search) ||
                t.Artist.Name.Contains(search));
        }

        var totalCount = await query.CountAsync();

        var tracks = await query
            .OrderBy(t => t.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (tracks, totalCount);
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

    // Could potentially be moved to a soft delete if it's deemed important to not hard delete or lots of hard deletes causing performance issues
    public async Task DeleteAsync(Track track, CancellationToken cancellationToken)
    {
        _context.TrackGenres.RemoveRange(track.TrackGenres);
        _context.Tracks.Remove(track);

        await _context.SaveChangesAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using Uppbeat.Api.Data;

namespace Uppbeat.Api.Repositories;

public class ArtistRepository : IArtistRepository
{
    private readonly UppbeatDbContext _context;

    public ArtistRepository(UppbeatDbContext context)
    {
        _context = context;
    }

    public async Task<Artist?> GetByNameAsync(string artistName, CancellationToken cancellationToken)
    {
        return await _context.Artists.FirstOrDefaultAsync(a => a.Name == artistName, cancellationToken);
    }

    public async Task<Artist> CreateAsync(Artist artist, CancellationToken cancellationToken)
    {
        _context.Artists.Add(artist);

        await _context.SaveChangesAsync(cancellationToken);

        return artist;
    }
}

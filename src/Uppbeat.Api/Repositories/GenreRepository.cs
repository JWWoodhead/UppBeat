using Microsoft.EntityFrameworkCore;
using Uppbeat.Api.Data;

namespace Uppbeat.Api.Repositories;

public class GenreRepository : IGenreRepository
{
    private UppbeatDbContext _context;

    public GenreRepository(UppbeatDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Genre>> GetGenresByNames(IEnumerable<string> genreNames, CancellationToken cancellationToken)
    {
        return await _context.Genres
            .Where(g => genreNames.Contains(g.Name))
            .ToListAsync(cancellationToken);
    }
}

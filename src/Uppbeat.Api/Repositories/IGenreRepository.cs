using Uppbeat.Api.Data;

namespace Uppbeat.Api.Repositories;

public interface IGenreRepository
{
    Task<IEnumerable<Genre>> GetGenresByNames(IEnumerable<string> genreNames, CancellationToken cancellationToken);
}

using Uppbeat.Api.Common;
using Uppbeat.Api.Models.Track;

namespace Uppbeat.Api.Services;

public interface ITrackService
{
    Task<Result<GetTrackResponse>> GetTrackByIdAsync(int id, CancellationToken cancellationToken);

    Task<GetTracksResponse> GetTracksAsync(GetTracksRequest request, CancellationToken cancellationToken);

    Task<Result<CreateTrackResponse>> CreateTrackAsync(CreateTrackRequest createTrackRequest, CancellationToken cancellationToken);

    Task<Result> UpdateTrackAsync(int id, int artistId, UpdateTrackRequest updateTrackRequest, CancellationToken cancellationToken);

    Task<Result> DeleteTrackAsync(int id, int artistId, CancellationToken cancellationToken);
}

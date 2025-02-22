using Uppbeat.Api.Common;
using Uppbeat.Api.Models.Track;

namespace Uppbeat.Api.Services;

public interface ITrackService
{
    Task<Result<CreateTrackResponse>> CreateTrackAsync(CreateTrackRequest createTrackRequest, CancellationToken cancellationToken);

    Task<Result<GetTrackResponse>> GetTrackByIdAsync(int id, CancellationToken cancellationToken);
}

using Uppbeat.Api.Data;
using Uppbeat.Api.Models.Auth;

public interface IUserService
{
    Task<LoginUserResponse> LoginUserAsync(UppbeatUser user, CancellationToken cancellationToken);
}
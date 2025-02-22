namespace Uppbeat.Api.Models.Auth;

public class LoginUserResponse
{
    /// <summary>
    /// Token for the newly authenticated user.
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Expiry date for the token.
    /// </summary>
    public DateTime Expiration { get; set; }

    /// <summary>
    /// Username of the authenticated user.
    /// </summary>
    public string Username { get; set; }
}

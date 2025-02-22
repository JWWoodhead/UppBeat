namespace Uppbeat.Api.Models.Auth;

public class LoginUserRequest
{
    /// <summary>
    /// Username to login as.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Password associated with user.
    /// </summary>
    public string Password { get; set; }
}

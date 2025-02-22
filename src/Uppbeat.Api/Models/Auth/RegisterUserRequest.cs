namespace Uppbeat.Api.Models.Auth;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// User details associated with registering a new user.
/// </summary>
public class RegisterUserRequest
{
    /// <summary>
    /// Username for the new user
    /// </summary>
    [Required]
    public string Username { get; set; }

    /// <summary>
    /// Email address for the new user
    /// </summary>
    [Required]
    public string Email { get; set; }

    /// <summary>
    /// Password for the new user
    /// </summary>
    [Required]
    public string Password { get; set; }

    /// <summary>
    /// Firt name of the user
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Last name of the user
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Artist name of the associated user. Specify this to give the user artist privledges
    /// </summary>
    public string? ArtistName { get; set; }
}

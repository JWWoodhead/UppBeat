using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Uppbeat.Api.Data;
using Uppbeat.Api.Models.Auth;
using Uppbeat.Api.Services;

namespace Uppbeat.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly UserManager<UppbeatUser> _userManager;
    private readonly SignInManager<UppbeatUser> _signInManager;
    private readonly IUserService _userService;
    private readonly IArtistService _artistService;

    public AuthController(
        UserManager<UppbeatUser> userManager,
        SignInManager<UppbeatUser> signInManager,
        IUserService userService,
        IArtistService artistService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userService = userService;
        _artistService = artistService;
    }

    /// <summary>
    /// Registers a new user using the provided registration details.
    /// </summary>
    /// <param name="registerUserRequest">User details to register</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
    /// <returns>
    /// Returns a success message upon successful registration. If the user already exists or registration fails, returns an appropriate error message.
    /// </returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest registerUserRequest, CancellationToken cancellationToken)
    {
        var userExists = await _userManager.FindByNameAsync(registerUserRequest.Username);

        if (userExists != null)
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "User already exists. Please try another username");

        var user = new UppbeatUser
        {
            Email = registerUserRequest.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = registerUserRequest.Username,
        };

        var result = await _userManager.CreateAsync(user, registerUserRequest.Password);

        // Should probably be outside the controller but no time!
        if (!string.IsNullOrEmpty(registerUserRequest.ArtistName))
        {
            var artist = await _artistService.GetByNameAsync(registerUserRequest.ArtistName, cancellationToken) 
                ?? await _artistService.CreateAsync(registerUserRequest.ArtistName, cancellationToken);

            var artistClaim = new Claim("ArtistId", artist.Id.ToString());
            await _userManager.AddClaimAsync(user, artistClaim);
        }

        if (!result.Succeeded)
            return Problem(
                detail: string.Join(Environment.NewLine, result.Errors),
                statusCode: StatusCodes.Status500InternalServerError,
                title: "User creation failed. Please check user details and try again.");

        return Ok();
    }
    
    /// <summary>
    /// Authenticates a user based on provided credentials and returns a JWT token upon successful login.
    /// </summary>
    /// <param name="loginUserRequest">Login details including username and password.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
    /// <returns>
    /// Returns a JWT token if authentication is successful; otherwise, returns an error message indicating invalid credentials.
    /// </returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest loginUserRequest, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(loginUserRequest.Username);
        if (user == null)
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Invalid username");

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginUserRequest.Password, false);

        if (!result.Succeeded)
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Invalid password");

        var loginResponse = await _userService.LoginUserAsync(user, cancellationToken);

        return Ok(loginResponse);
    }
}

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Uppbeat.Api.Data;
using Uppbeat.Api.Models.Auth;

namespace Uppbeat.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly UserManager<UppbeatUser> _userManager;

    public AuthController(
        UserManager<UppbeatUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Registers a new user using the provided registration details.
    /// </summary>
    /// <param name="registerUserRequest">User details to register</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
    /// Returns a success message upon successful registration. If the user already exists or registration fails, returns an appropriate error message.
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

        if (!result.Succeeded)
            return Problem(
                detail: string.Join(Environment.NewLine, result.Errors),
                statusCode: StatusCodes.Status500InternalServerError,
                title: "User creation failed. Please check user details and try again.");

        return Ok();
    }
}

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uppbeat.Api.Common;
using Uppbeat.Api.Models.Track;
using Uppbeat.Api.Services;

namespace Uppbeat.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class TracksController : ControllerBase
{
    private readonly ITrackService _trackService;

    public TracksController(ITrackService trackService)
    {
        _trackService = trackService;
    }

    /// <summary>
    /// Retrieves the details of a specific track by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the track.</param>
    /// <param name="cancellationToken">Cancellation token assocaited with the request</param>
    /// <returns>
    /// Returns a 200 OK response with the track details if found,
    /// otherwise returns a 404 Not Found response.
    /// </returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetTrackResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTrack(int id, CancellationToken cancellationToken)
    {
        var result = await _trackService.GetTrackByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return Problem(
                detail: result.Error,
                statusCode: result.StatusCode,
                title: "Failed to get track");

        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new track for the currently logged in artist user.
    /// </summary>
    /// <param name="createTrackRequest">Details of the track to be created.</param>
    /// <param name="cancellationToken">Cancellation token assocaited with the request</param>
    /// <returns>
    /// Returns a 201 Created response with the newly created track details 
    /// and a Location header pointing to the new resource.
    /// Returns a 400 Bad Request if the submitted data is invalid.
    /// </returns>
    [HttpPost]
    [Authorize(Policy = CustomPolicies.IsArtist)]
    [ProducesResponseType(typeof(CreateTrackResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateTrack([FromBody] CreateTrackRequest createTrackRequest, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _trackService.CreateTrackAsync(createTrackRequest, cancellationToken);

        if (!result.IsSuccess)
            return Problem(
                detail: result.Error,
                statusCode: result.StatusCode,
                title: "Failed to create track");

        return CreatedAtAction(nameof(GetTrack), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Updates the details of an existing track for the currently logged in artist user.
    /// </summary>
    /// <param name="id">The unique identifier of the track to update.</param>
    /// <param name="updateTrackRequest">The data to update the existing track with.</param>
    /// <param name="cancellationToken">Cancellation token assocaited with the request</param>
    /// <returns>
    /// Returns a 204 No Content response if the track is successfully updated.
    /// Returns a 404 Not Found response if the track does not exist.
    /// </returns>
    [HttpPut("{id}")]
    [Authorize(Policy = CustomPolicies.IsArtist)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateTrack(int id, [FromBody] UpdateTrackRequest updateTrackRequest, CancellationToken cancellationToken)
    {
        var artistId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "ArtistId")!.Value);

        var result = await _trackService.UpdateTrackAsync(id, artistId, updateTrackRequest, cancellationToken);

        if (!result.IsSuccess)
            return Problem(
                detail: result.Error,
                statusCode: result.StatusCode,
                title: "Failed to update track");

        return NoContent();
    }
}

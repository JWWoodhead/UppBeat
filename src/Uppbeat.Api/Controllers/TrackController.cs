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

        return Created("", result.Value);
        // Need to update to CreatedAtAction when GetTrack is implemented
        // return CreatedAtAction(nameof(GetTrack), new { id = result.Value!.Id }, result.Value);
    }
}

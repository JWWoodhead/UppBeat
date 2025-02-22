using System.ComponentModel.DataAnnotations;

namespace Uppbeat.Api.Models.Track;

public class GetTracksRequest
{
    /// <summary>
    /// Page number being requested
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1.")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Max number of items to be returned for a single request
    /// </summary>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Genre to filter on.
    /// </summary>
    public string? Genre { get; set; }

    /// <summary>
    /// Free text search for filtering by artist or track name.
    /// </summary>
    public string? Search { get; set; }
}

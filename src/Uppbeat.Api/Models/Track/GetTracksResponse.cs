namespace Uppbeat.Api.Models.Track;

/// <summary>
/// Represents a paginated response containing multiple tracks.
/// </summary>
public class GetTracksResponse
{
    /// <summary>
    /// The current page number.
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// The total number of tracks matching the query (across all pages).
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// The collection of tracks for the current page.
    /// </summary>
    public IEnumerable<GetTracksItemResponse> Tracks { get; init; } = new List<GetTracksItemResponse>();
}

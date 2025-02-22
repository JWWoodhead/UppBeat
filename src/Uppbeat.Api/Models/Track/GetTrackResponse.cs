namespace Uppbeat.Api.Models.Track;

/// <summary>
/// Represents the data returned when retrieving a specific track.
/// </summary>
public class GetTrackResponse
{
    /// <summary>
    /// The unique identifier of the track.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// The name/title of the track.
    /// </summary>
    public string Name { get; init; } = default!;

    /// <summary>
    /// The unique identifier of the artist associated with this track.
    /// </summary>
    public int ArtistId { get; init; }

    /// <summary>
    /// The duration of the track, in seconds.
    /// </summary>
    public int Duration { get; init; }

    /// <summary>
    /// The file name or path of the audio file.
    /// </summary>
    public string File { get; init; } = default!;

    /// <summary>
    /// The list of genres associated with the track.
    /// </summary>
    public List<string> Genres { get; init; } = new();
}

namespace Uppbeat.Api.Models.Track;

/// <summary>
/// Represents an item in a list of tracks, including basic details.
/// </summary>
public class GetTracksItemResponse
{
    /// <summary>
    /// The unique identifier of the track.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// The name/title of the track.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// The file name or path for the audio file.
    /// </summary>
    public string File { get; init; }

    /// <summary>
    /// The duration of the track, in seconds.
    /// </summary>
    public int Duration { get; init; }

    /// <summary>
    /// The unique identifier of the artist.
    /// </summary>
    public int Artist { get; init; }

    /// <summary>
    /// The name of the artist.
    /// </summary>
    public string ArtistName { get; init; }

    /// <summary>
    /// The list of genres associated with this track.
    /// </summary>
    public IEnumerable<string> Genres { get; init; }
}

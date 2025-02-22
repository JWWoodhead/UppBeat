using System.ComponentModel.DataAnnotations;

namespace Uppbeat.Api.Models.Track;

/// <summary>
/// Represents the data required to update an existing track.
/// </summary>
public class UpdateTrackRequest
{
    /// <summary>
    /// The updated name/title of the track.
    /// </summary>
    [Required]
    public string Name { get; set; } = default!;

    /// <summary>
    /// The updated duration of the track, in seconds.
    /// </summary>
    [Required]
    public int Duration { get; set; }

    /// <summary>
    /// The updated file name or path of the audio file.
    /// </summary>
    [Required]
    public string File { get; set; } = default!;

    /// <summary>
    /// The updated list of genres associated with the track.
    /// </summary>
    [Required]
    public List<string> Genres { get; set; } = new();
}

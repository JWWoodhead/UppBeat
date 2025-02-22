using System.ComponentModel.DataAnnotations;

namespace Uppbeat.Api.Models.Track;

/// <summary>
/// Represents the data required to create a new track.
/// </summary>
public class CreateTrackRequest
{
    /// <summary>
    /// The name/title of the track.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; init; }

    /// <summary>
    /// The unique identifier of the artist creating the track.
    /// </summary>
    [Required]
    public int Artist { get; init; }

    /// <summary>
    /// The duration of the track, in seconds.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int Duration { get; init; }

    /// <summary>
    /// The file name or path for the audio file.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string File { get; init; }

    /// <summary>
    /// The list of genres associated with the track.
    /// </summary>
    [Required]
    public List<string> Genres { get; init; }
}

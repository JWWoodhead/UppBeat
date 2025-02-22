using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Uppbeat.Api.Models.Track;

namespace Uppbeat.Api.Data;

public class Track
{
    public Track() { }

    public Track(CreateTrackRequest createTrackRequest, IEnumerable<Genre> genres)
    {
        Name = createTrackRequest.Name;
        ArtistId = createTrackRequest.Artist;
        Duration = createTrackRequest.Duration;
        File = createTrackRequest.File;
        TrackGenres = genres
            .Select(genre => new TrackGenre 
            {
                Genre = genre,
                GenreId = genre.Id,
            })
            .ToList();
    }

    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    [Required]
    [ForeignKey(nameof(Artist))]
    public int ArtistId { get; set; }

    [Required]
    public int Duration { get; set; }  // in seconds

    [Required]
    public string File { get; set; } = default!;

    public virtual Artist Artist { get; set; } = default!;

    public virtual ICollection<TrackGenre> TrackGenres { get; set; } = new List<TrackGenre>();

    public void Update(UpdateTrackRequest trackUpdate, IEnumerable<Genre> matchingGenres)
    {
        Name = trackUpdate.Name;
        Duration = trackUpdate.Duration;
        File = trackUpdate.File;

        TrackGenres = matchingGenres
            .Select(genre => new TrackGenre 
            { 
                TrackId = Id, 
                Genre = genre, 
                GenreId = genre.Id 
            })
            .ToList();
    }
}

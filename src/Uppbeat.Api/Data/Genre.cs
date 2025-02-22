using System.ComponentModel.DataAnnotations;

namespace Uppbeat.Api.Data;

public class Genre
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    public virtual ICollection<TrackGenre> TrackGenres { get; set; } = new List<TrackGenre>();
}

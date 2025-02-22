using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uppbeat.Api.Data;

public class TrackGenre
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey(nameof(Track))]
    public int TrackId { get; set; }

    [Required]
    [ForeignKey(nameof(Genre))]
    public int GenreId { get; set; }

    public virtual Track Track { get; set; } = default!;
    public virtual Genre Genre { get; set; } = default!;
}

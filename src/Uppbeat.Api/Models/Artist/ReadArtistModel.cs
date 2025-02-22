using System.ComponentModel.DataAnnotations;

namespace Uppbeat.Api.Models.Artist;

public class ReadArtistModel
{
    [Key]
    public int Id { get; init; }

    [Required]
    public string Name { get; init; } = default!;
}

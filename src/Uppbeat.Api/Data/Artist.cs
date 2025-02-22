using System.ComponentModel.DataAnnotations;

namespace Uppbeat.Api.Data;

public class Artist
{
    public Artist(string name)
    {
        Name = name;
    }

    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;
}

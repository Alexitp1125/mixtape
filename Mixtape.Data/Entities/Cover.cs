using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Mixtape.Data.Entities
{
    // A cover: an SVG file of Content/covers. The file stays in the repo; this row says which songs use it.
    [Table("Covers")]
    [Index(nameof(Name), IsUnique = true)]
    public sealed class Cover
    {
        [Key]
        [Column("Key")]
        public Guid Key { get; set; }

        [Required]
        [StringLength(80)]
        [Column("Name")]
        public string Name { get; set; } = null!;

        // File name inside Content/covers, for example "doma.svg".
        [Required]
        [StringLength(120)]
        [Column("FileName")]
        public string FileName { get; set; } = null!;

        // True for the covers any song can use; false for the one drawn for a single song.
        [Column("IsGeneric")]
        public bool IsGeneric { get; set; }

        public List<Track> Tracks { get; set; } = [];
    }
}

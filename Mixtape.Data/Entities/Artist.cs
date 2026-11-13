using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Mixtape.Data.Entities
{
    // An artist of the catalog, with their songs.
    [Table("Artists")]
    [Index(nameof(Slug), IsUnique = true)]
    public sealed class Artist
    {
        [Key]
        [Column("Key")]
        public Guid Key { get; set; }

        // Name used in addresses and in catalog.json.
        [Required]
        [StringLength(80)]
        [Column("Slug")]
        public string Slug { get; set; } = null!;

        [Required]
        [StringLength(120)]
        [Column("Name")]
        public string Name { get; set; } = null!;

        // Position in the catalog.
        [Column("SortOrder")]
        public int SortOrder { get; set; }

        public List<Track> Tracks { get; set; } = [];
    }
}

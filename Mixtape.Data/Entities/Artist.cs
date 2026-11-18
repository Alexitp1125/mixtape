using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Mixtape.Data.Entities
{
    // An artist of the catalog or the mixtape of a person, with its cover and its songs.
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

        // Empty when the artist has no cover.
        [Column("CoverKey")]
        public Guid? CoverKey { get; set; }

        [ForeignKey(nameof(CoverKey))]
        public Cover? Cover { get; set; }

        // The person the mixtape belongs to. Empty for the artists of the catalog.
        [Column("UserKey")]
        public Guid? UserKey { get; set; }

        [ForeignKey(nameof(UserKey))]
        public User? User { get; set; }

        public List<Track> Tracks { get; set; } = [];
    }
}

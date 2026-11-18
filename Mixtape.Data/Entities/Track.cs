using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Mixtape.Data.Entities
{
    // A song: a YouTube video with its artist and its cover.
    [Table("Tracks")]
    [Index(nameof(Slug), IsUnique = true)]
    public sealed class Track
    {
        [Key]
        [Column("Key")]
        public Guid Key { get; set; }

        [Column("ArtistKey")]
        public Guid ArtistKey { get; set; }

        [ForeignKey(nameof(ArtistKey))]
        public Artist Artist { get; set; } = null!;

        // Empty when the song has no cover.
        [Column("CoverKey")]
        public Guid? CoverKey { get; set; }

        [ForeignKey(nameof(CoverKey))]
        public Cover? Cover { get; set; }

        // Name used in the address of the song: /tracks/{slug}.
        [Required]
        [StringLength(80)]
        [Column("Slug")]
        public string Slug { get; set; } = null!;

        [Required]
        [StringLength(120)]
        [Column("Title")]
        public string Title { get; set; } = null!;

        // Who performs the song. Empty when it is the artist the song belongs to.
        [StringLength(120)]
        [Column("Performer")]
        public string? Performer { get; set; }

        // YouTube video ID (the part after v=).
        [Required]
        [StringLength(20)]
        [Column("VideoId")]
        public string VideoId { get; set; } = null!;

        // Position among the songs of its artist.
        [Column("SortOrder")]
        public int SortOrder { get; set; }
    }
}

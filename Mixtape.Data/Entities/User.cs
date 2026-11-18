using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Mixtape.Data.Entities
{
    // A person with an account: a name and a PIN. Nothing else is stored about them.
    [Table("Users")]
    [Index(nameof(NameKey), IsUnique = true)]
    public sealed class User
    {
        [Key]
        [Column("Key")]
        public Guid Key { get; set; }

        // The name as the person wrote it.
        [Required]
        [StringLength(30)]
        [Column("Name")]
        public string Name { get; set; } = null!;

        // The name in lower case: two names that only differ in capitals are the same account.
        [Required]
        [StringLength(30)]
        [Column("NameKey")]
        public string NameKey { get; set; } = null!;

        // The PIN after hashing. The PIN itself is never stored.
        [Required]
        [StringLength(200)]
        [Column("PinHash")]
        public string PinHash { get; set; } = null!;

        // Wrong PINs in a row.
        [Column("FailedCount")]
        public int FailedCount { get; set; }

        // The account accepts no PIN until this moment (UTC). Empty when it isn't locked.
        [Column("LockedUntil")]
        public DateTime? LockedUntil { get; set; }

        // When the account was created (UTC).
        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}

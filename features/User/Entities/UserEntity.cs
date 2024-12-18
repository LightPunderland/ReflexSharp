using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Features.User.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Features.User.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(DisplayName), IsUnique = true)]
    public class User : IComparable<User>, IEquatable<User>
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, Column(TypeName = "varchar(255)")]
        public string GoogleId { get; set; } = null!;

        [Required, Column(TypeName = "varchar(255)")]
        public string Email { get; set; } = null!;

        [Required, Column(TypeName = "varchar(100)")]
        public string DisplayName { get; set; } = null!;

        [Required]
        [Column("Rank")]
        public Rank Rank { get; set; } = Rank.None;

        [Required]
        [Column("XP")]
        public Int32 XP { get; set; } = 0;

        [Required]
        [Column("Gold")]
        public int Gold { get; set; } = 0;

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string EquippedSkin { get; set; } = "Ninja";

        [Column(TypeName = "text")]
        public string OwnedSkins { get; set; } = "Ninja";

        // Comapre by rank
        public int CompareTo(User? other)
        {
            if (other == null)
                return 1;
            return Rank.CompareTo(other.Rank);
        }

        // Equal if name and rank are equal
        public bool Equals(User? other)
        {
            if (other == null)
                return false;

            return this.DisplayName == other.DisplayName;
        }

        public override bool Equals(object? obj)
        {
            if (obj is User otherUser)
            {
                return Equals(otherUser);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DisplayName, Rank);
        }
    }
}

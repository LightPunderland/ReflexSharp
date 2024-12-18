using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Features.User.DTOs; 

namespace Features.Wardrobe.Entities
{
    public class WardrobeItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; } = null!;

        [Required]
        public int Price { get; set; }

        [Required]
        public Rank RequiredRank { get; set; } = Rank.None;
    }
}